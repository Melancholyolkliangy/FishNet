Shader "Unity Shaders Book/Chapter7-Mask Texture"
{
    Properties
    {
        _Color("Color Tint",Color) = (1,1,1,1)
        _MainTex("Main Tex", 2D) = "white"{}
        _BumpMap("Bump Map", 2D) = "bump"{}
        _BumpMapScale("Bump Scale", Float) = 1
        _SpecularMask("Specular Mask", 2D) = "white"{}
        _SpecularScale("Specular Scale",Float) = 1
        _Specular("Specular",Color) = (1,1,1,1)
        _Gloss("Gloss",Range(8,256)) = 20
    }
    SubShader
    {
        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BumpMap;
            float _BumpMapScale;
            fixed4 _Specular;
            sampler2D _SpecularMask;
            float _SpecularScale;
            float _Gloss;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 TtoW0 : TEXCOORD1;
                float4 TtoW1 : TEXCOORD2;
                float4 TtoW2 : TEXCOORD3;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = mul(unity_MatrixMVP,v.vertex);
                o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                float3 worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                fixed3 worldBinomal = cross(worldNormal,worldTangent) * v.tangent.w;
                
                o.TtoW0 = float4(worldTangent.x,worldBinomal.x,worldNormal.x,worldPos.x);
                o.TtoW1 = float4(worldTangent.y,worldBinomal.y,worldNormal.y,worldPos.y);
                o.TtoW2 = float4(worldTangent.z,worldBinomal.z,worldNormal.z,worldPos.z);
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 worldPos = float3(i.TtoW0.w,i.TtoW1.w,i.TtoW2.w);
                
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                fixed4 packedNormal = tex2D(_BumpMap, i.uv);
                fixed3 bump = UnpackNormal(packedNormal);
                bump.xy *= _BumpMapScale;
                bump.z = sqrt(1 - saturate(dot(bump.xy,bump.xy)));
                bump = normalize(half3(dot(i.TtoW0.xyz,bump),dot(i.TtoW1.xyz,bump),dot(i.TtoW2,bump)));
                
                fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                
                fixed3 diffuse = _LightColor0 * albedo * saturate(dot(bump, worldLightDir));
                fixed3 h = normalize(worldLightDir + worldViewDir);
                fixed specularMask = tex2D(_SpecularMask, i.uv).r * _SpecularScale;
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(h, bump)),_Gloss) * specularMask;
                fixed3 color = ambient + diffuse + specular;
                return fixed4(color, 1);
            }
            ENDCG
        }
    }
    Fallback "Specular"
}
