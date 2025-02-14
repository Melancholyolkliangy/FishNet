Shader "Unity Shaders Book/Chapter7-NormalMapTangentSpace"
{
    Properties
    {
        _Color("Color Tint",Color) = (1,1,1,1)
        _MainTex("Main Tex", 2D) = "white"{}
        _BumpMap("Bump Map", 2D) = "bump"{}
        _BumpMapScale("Bump Scale", Float) = 1
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
            float4 _BumpMap_ST;
            fixed4 _Specular;
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
                float4 uv : TEXCOORD2;
                float3 lightDir : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = mul(unity_MatrixMVP,v.vertex);
                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;
                float3 binormal = cross(normalize(v.normal),normalize(v.tangent.xyz) * v.tangent.w);
                float3x3 tangentTransformer = float3x3(v.tangent.xyz,binormal,v.normal);
                //TANGENT_SPACE_ROTATION
                o.lightDir = mul(tangentTransformer,ObjSpaceLightDir(v.vertex)).xyz;
                o.viewDir = mul(tangentTransformer,ObjSpaceViewDir(v.vertex)).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 tangentLightDir = normalize(i.lightDir);
                fixed3 tangentViewDir = normalize(i.viewDir);
                fixed4 packedNormal = tex2D(_BumpMap, i.uv.zw);
                fixed3 tangentNormal;
                tangentNormal = UnpackNormal(packedNormal);
                tangentNormal.xy *= _BumpMapScale;
                tangentNormal.z = sqrt(1 - saturate(dot(tangentNormal.xy,tangentNormal.xy)));
                
                fixed3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                
                fixed3 diffuse = _LightColor0 * albedo * saturate(dot(tangentNormal, tangentLightDir));
                fixed3 h = normalize(tangentLightDir + tangentViewDir);
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(h, tangentNormal)),_Gloss);
                fixed3 color = ambient + diffuse + specular;
                return fixed4(color, 1);
            }
            ENDCG
        }
    }
    Fallback "Specular"
}
