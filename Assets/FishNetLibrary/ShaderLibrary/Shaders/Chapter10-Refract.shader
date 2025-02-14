// Upgrade NOTE: replaced 'defined USING_DIRECTIONAL_LIGHT' with 'defined (USING_DIRECTIONAL_LIGHT)'

Shader"Unity Shaders Book/Chapter10-Refract"
{
    Properties
    {
        _Color("Color Tint", Color) = (1,1,1,1)
        _RefractColor("Refraction Color", Color) = (1,1,1,1)
        _RefractAmount("Refraction Amount",Range(0,1)) = 1
        _RefractRatio("Refraction Ratio",Range(0,1)) = 1
        _Cubemap("Refraction Cubemap",Cube) = "_Skybox"{}
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
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            fixed4 _Color;
            fixed4 _RefractColor;
            fixed _RefractAmount;
            fixed _RefractRatio;
            samplerCUBE _Cubemap;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed3 worldNormal: TEXCOORD0;
                fixed3 worldPosition: TEXCOORD1;
                fixed3 worldViewDir : TEXCOORD2;
                fixed3 worldRefract : TEXCOORD3;
                SHADOW_COORDS(4)
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = mul(unity_MatrixMVP,v.vertex);
                o.worldPosition = mul(unity_ObjectToWorld,v.vertex);
                o.worldNormal = mul(unity_ObjectToWorld,v.normal);
                o.worldViewDir = UnityWorldSpaceViewDir(o.worldPosition);
                o.worldRefract = refract(-normalize(o.worldViewDir), normalize(o.worldNormal),_RefractRatio);
                TRANSFER_SHADOW(o)
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 diffuse = _LightColor0 * _Color.rgb * saturate(dot(worldNormal, worldLight));
                fixed3 refraction = texCUBE(_Cubemap,i.worldRefract).rgb * _RefractColor.rgb;
                UNITY_LIGHT_ATTENUATION(atten,i ,i.worldPosition);
                fixed3 color = ambient + lerp(diffuse, refraction, _RefractAmount) * atten;
                return fixed4(color, 1);
            }
            ENDCG
        }
        Pass
        {
            Tags
            {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            CGPROGRAM

            #pragma multi_compile_fwdadd
            #pragma shader_feature_local _MAIN_LIGHT_SHADOWS
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"
            #include "UnityCG.cginc"

            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;
            uniform float4x4 unity_WorldToLight;
            sampler2D _LightTexture0;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed3 worldNormal: TEXCOORD0;
                fixed3 worldPosition: TEXCOORD1;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = mul(unity_MatrixMVP,v.vertex);
                o.worldPosition = mul(unity_ObjectToWorld,v.vertex);
                o.worldNormal = mul(unity_ObjectToWorld,v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                fixed3 worldNormal = normalize(i.worldNormal);
                #ifdef USING_DIRCTIONAL_LIGHT
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                #else
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz - i.worldPosition);
                #endif
                fixed3 diffuse = _LightColor0 * _Diffuse.rgb * saturate(dot(worldNormal, worldLight));
                fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPosition);
                fixed3 h = normalize(worldLight + viewDir);
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(h, worldNormal)),_Gloss);
                #if defined (SPOT)
                fixed atten = 1;
                #elif defined (POINT)
                fixed atten = 1;
                #else
                fixed atten = 1;
                
                #endif
                
                fixed3 color = ambient + (diffuse + specular) * atten;
                return fixed4(color, 1);
            }
            
            ENDCG
        }
    }
    Fallback "Specular"
}
