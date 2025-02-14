Shader "Unity Shaders Book/Chapter8-AlphaTest"
{
    Properties
    {
        _Color("Main Tint",Color) = (1,1,1,1)
        _MainTex("Main Tex", 2D) = "white"{}
        _Cutoff("Alpha Cutoff",Range(0,1)) = 0.5
    }
    SubShader
    {
        Pass
        {
            Tags
            {
                "Queue"="AlphaTest"
                "IgnoreProjector"="True"
                "RenderType"="TransparentCutout"
                "LightMode"="ForwardBase"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Cutoff;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPosition : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = mul(unity_MatrixMVP,v.vertex);
                o.worldPosition = mul(unity_ObjectToWorld,v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv).rgba;
                clip(texColor.a - _Cutoff);
                fixed4 albedo = texColor * _Color.rgba;
                fixed4 ambient = UNITY_LIGHTMODEL_AMBIENT.xyzw * albedo;
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLight = normalize(UnityWorldSpaceLightDir(i.worldPosition));
                fixed4 diffuse = _LightColor0 * albedo * saturate(dot(worldNormal, worldLight));
                return fixed4(ambient + diffuse);
            }
            ENDCG
        }
    }
    Fallback "Specular"
}
