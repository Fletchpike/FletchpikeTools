Shader "Hidden/Fletchpike/EdgeDetect"
{
    HLSLINCLUDE
        // StdLib.hlsl holds pre-configured vertex shaders (VertDefault), varying structs (VaryingsDefault), and most of the data you need to write common effects.
#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        sampler2D _CameraDepthTexture;
        sampler2D _CameraDepthNormalsTexture;

    float4 _EdgeColor;
    float _OutlineThickness;
    float _Sensitivity;

    // Edge detection kernel that works by taking the sum of the squares of the differences between diagonally adjacent pixels (Roberts Cross).
    float RobertsCross(float3 samples[4])
    {
        const float3 difference_1 = samples[1] - samples[2];
        const float3 difference_2 = samples[0] - samples[3];
        return sqrt(dot(difference_1, difference_1) + dot(difference_2, difference_2));
    }

    // The same kernel logic as above, but for a single-value instead of a vector3.
    float RobertsCross(float samples[4])
    {
        const float difference_1 = samples[1] - samples[2];
        const float difference_2 = samples[0] - samples[3];
        return sqrt(difference_1 * difference_1 + difference_2 * difference_2);
    }

    // Helper function to sample scene normals remapped from [-1, 1] range to [0, 1].
    float4 SampleSceneNormalsRemapped(float2 uv)
    {
        float4 normal = tex2D(_CameraDepthNormalsTexture, uv);
        return normal;
    }
    half SampleSceneNormalStrength(float2 uv)
    {
        half4 normal = tex2D(_CameraDepthNormalsTexture, uv);
        half str = 1;
        half dir = dot(normal, float3(0, 0, 1));
        str = lerp(1, 0, abs(dir));
        return str;
    }

    // Helper function to sample scene luminance.
    float SampleSceneLuminance(float2 uv)
    {
        float3 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        return color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
    }
    
    float SampleSceneDepth(float2 uv)
    {
        float3 depth = tex2D(_CameraDepthTexture, uv);
        return depth.r;
    }


    half4 Frag(VaryingsDefault IN) : SV_Target
    {
        // Screen-space coordinates which we will use to sample.
        float2 uv = IN.texcoord;
        float2 texel_size = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);
        half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

        // Generate 4 diagonally placed samples.
        const float half_width_f = floor(_OutlineThickness * 0.5);
        const float half_width_c = ceil(_OutlineThickness * 0.5);

        float2 uvs[4];
        uvs[0] = uv + texel_size * float2(half_width_f, half_width_c) * float2(-1, 1);  // top left
        uvs[1] = uv + texel_size * float2(half_width_c, half_width_c) * float2(1, 1);   // top right
        uvs[2] = uv + texel_size * float2(half_width_f, half_width_f) * float2(-1, -1); // bottom left
        uvs[3] = uv + texel_size * float2(half_width_c, half_width_f) * float2(1, -1);  // bottom right

        //float3 normal_samples[4];
        float depth_samples[4], luminance_samples[4];

        for (int i = 0; i < 4; i++) {
            depth_samples[i] = SampleSceneDepth(uvs[i]);
            //normal_samples[i] = SampleSceneNormalsRemapped(uvs[i]);
            luminance_samples[i] = SampleSceneLuminance(uvs[i]);
        }

        // Apply edge detection kernel on the samples to compute edges.
        float edge_depth = RobertsCross(depth_samples);
        //float edge_normal = RobertsCross(normal_samples);
        float edge_luminance = RobertsCross(luminance_samples);

        // Threshold the edges (discontinuity must be above certain threshold to be counted as an edge). The sensitivities are hardcoded here.
        float depth_threshold = 1 / _Sensitivity;
        edge_depth = edge_depth > depth_threshold ? 1 : 0;

        //float normal_threshold = 1 / 4.0f;
        //edge_normal = edge_normal > normal_threshold ? 1 : 0;

        float luminance_threshold = 1 / 0.5f;
        edge_luminance = edge_luminance > luminance_threshold ? 1 : 0;

        // Combine the edges from depth/normals/luminance using the max operator.
        float edge = max(edge_depth, edge_luminance);
        //edge *= SampleSceneNormalStrength(uv);
        // Color the edge with a custom color.
        return lerp(color, _EdgeColor, edge);
    }
        ENDHLSL
        SubShader
    {
        Cull Off ZWrite Off ZTest Always
            Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment Frag
            ENDHLSL
        }
    }
}
