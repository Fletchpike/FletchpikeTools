Shader "Unlit/Unlit Distance Dither"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _StartDistance("Start Distance", Float) = 10
        _Range("Smoothness", Float) = 1
        [ToggleUI] _Invert("Invert", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Range;
            float _StartDistance;
            float _Invert;

            static const float bayerMatrix[16] = {
                     1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                    13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                     4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                    16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                UNITY_APPLY_FOG(i.fogCoord, col);

                float3 worldPos = i.worldPos;
                float distanceToCam = distance(_WorldSpaceCameraPos, worldPos);
                float alp = saturate((_StartDistance - distanceToCam) / _Range);
                if (_Invert)
                {
                    alp = 1 - alp;
                }
                float targetAlpha = col.a * alp;
                float2 uv = i.vertex.xy * _ScreenParams.xy;
                uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
                    clip(targetAlpha - bayerMatrix[index]);
                    return col;
                }
            ENDCG
        }
    }
}
