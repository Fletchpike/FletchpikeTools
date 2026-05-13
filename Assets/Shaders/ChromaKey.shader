Shader "UI/Chroma Key"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Key("Key", Color) = (0, 1, 0, 1)
        _Slope("Slope", Range(0, 1)) = 0.2
        _Threshold("Threshold", Range(0, 2)) = 0.8
        [ToggleUI]_Cutoff("Cutoff", Float) = 0
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent"
            "Queue"= "Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }
        Blend SrcAlpha OneMinusSrcAlpha
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Key;
            float _Slope;
            float _Threshold;
            float _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float4 value = abs(length(abs(_Key - col)));
                float edge = smoothstep((1 - _Slope) * _Threshold, _Threshold, value);
                col.a = _Cutoff == 1 ? step(0.001, edge) : edge;
                return col;
            }
            ENDCG
        }
    }
}
