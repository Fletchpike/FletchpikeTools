Shader "Hidden/Fletchpike/LinearDepthOfField"
{
    HLSLINCLUDE
        // StdLib.hlsl holds pre-configured vertex shaders (VertDefault), varying structs (VaryingsDefault), and most of the data you need to write common effects.
#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
#define SAMPLES 30.0
    sampler2D _CameraDepthTexture;
    float2 inch;
    float _BlurSize;
    float _MinDistance;
    float _MaxDistance;

    float4 GetFrom(float2 uv, float2 offset)
    {
        return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offset);
    }

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float2 uv = i.texcoord;
        float4 color = GetFrom(uv, float2(0, 0));
        float depth = Linear01Depth(tex2D(_CameraDepthTexture, uv).r) * _ProjectionParams.z;
        float2 size = _ScreenParams.xy;
        inch = float2(1, 1) / float2(640, 360) * _BlurSize;
        for (float i = 0; i < SAMPLES; i++)
        {
            float scalar = i / SAMPLES * 5;
            color += GetFrom(uv, float2(inch.x * scalar, 0));
            color += GetFrom(uv, float2(0, inch.y * scalar));
            color += GetFrom(uv, float2(-inch.x * scalar, 0));
            color += GetFrom(uv, float2(0, -inch.y * scalar));
            color += GetFrom(uv, float2(inch.x * scalar, inch.y * scalar));
            color += GetFrom(uv, float2(-inch.x * scalar, -inch.y * scalar));
            color += GetFrom(uv, float2(-inch.x * scalar, inch.y * scalar));
            color += GetFrom(uv, float2(inch.x * scalar, -inch.y * scalar));
        }
        float d = clamp(depth, _MinDistance, _MaxDistance) / _MaxDistance;
        color = normalize(color);
        //return float4(d, d, d, 1);
        return lerp(GetFrom(uv, float2(0, 0)), color, d);
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
