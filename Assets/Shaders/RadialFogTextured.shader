Shader "Custom/RadialFogTextured"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0,0,0,1)
        _FogStart ("Fog Start", Float) = 0.3
        _FogEnd ("Fog End", Float) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _FogColor;
            float _FogStart;
            float _FogEnd;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float dist = distance(_WorldSpaceCameraPos, i.worldPos);
                float t = saturate((dist - _FogStart) / (_FogEnd - _FogStart));
                float k = 3.5;
                float fogFactor = (1.0 - exp(-k * t)) / (1.0 - exp(-k));
                fogFactor = saturate(fogFactor);
                if (dist >= _FogEnd) fogFactor = 1.0;
                col.rgb = lerp(col.rgb, _FogColor.rgb, fogFactor);
                return col;
            }
            ENDCG
        }
    }
}
