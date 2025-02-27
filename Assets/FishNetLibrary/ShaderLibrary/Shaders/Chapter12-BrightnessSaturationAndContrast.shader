// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'defined USING_DIRECTIONAL_LIGHT' with 'defined (USING_DIRECTIONAL_LIGHT)'

Shader"Unity Shaders Book/Chapter12-BrightnessSaturationAndContrast"
{
    Properties
    {
        _MainTex("Base (RGB)",2D)="white"{}
        _Brightness("Brightness",Float) = 1
        _Saturation("Saturation",Float) = 1
        _Contrast("Contrast",Float) = 1
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }
            ZTest Always
            ZWrite Off
            Cull Off
            
            CGPROGRAM
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            half2 _MainTex_ST;
            half _Brightness;
            half _Saturation;
            half _Contrast;

            struct a2v
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = mul(unity_MatrixMVP,v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 renderTex = tex2D(_MainTex, i.uv);
                renderTex.rbga *= renderTex.rgba * _Brightness;

                fixed luminance = 0.125 * renderTex.r + 0.7154 * renderTex.g + 0.0721 * renderTex.b;
                fixed3 luminanceColor = fixed3(luminance,luminance,luminance);
                renderTex.rgb = lerp(luminanceColor,renderTex.rgba, _Saturation);

                fixed3 avgColor = fixed3(0.5,0.5,0.5);
                renderTex.rgb = lerp(avgColor,renderTex.rgba,_Contrast);
                return fixed4(renderTex.rgb, renderTex.a);
            }
            ENDCG
        }
//        Pass
//        {
//            Tags
//            {
//                "LightMode"="ForwardAdd"
//            }
//            Blend One One
//            CGPROGRAM
//            #pragma multi_compile_fwdadd
//            #pragma shader_feature_local _MAIN_LIGHT_SHADOWS
//            #pragma vertex vert
//            #pragma fragment frag
//
//            #include "Lighting.cginc"
//            #include "UnityCG.cginc"
//
//            fixed4 _Diffuse;
//            fixed4 _Specular;
//            float _Gloss;
//            uniform float4x4 unity_WorldToLight;
//            sampler2D _LightTexture0;
//
//            struct a2v
//            {
//                float4 vertex : POSITION;
//                float3 normal : NORMAL;
//            };
//
//            struct v2f
//            {
//                float4 pos : SV_POSITION;
//                fixed3 worldNormal: TEXCOORD0;
//                fixed3 worldPosition: TEXCOORD1;
//            };
//
//            v2f vert(a2v v)
//            {
//                v2f o;
//                o.pos = mul(unity_MatrixMVP, v.vertex);
//                o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
//                o.worldNormal = mul(unity_ObjectToWorld, v.normal);
//                return o;
//            }
//
//            fixed4 frag(v2f i) : SV_Target
//            {
//                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
//                fixed3 worldNormal = normalize(i.worldNormal);
//                #ifdef USING_DIRCTIONAL_LIGHT
//                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
//                #else
//                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz - i.worldPosition);
//                #endif
//                fixed3 diffuse = _LightColor0 * _Diffuse.rgb * saturate(dot(worldNormal, worldLight));
//                fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPosition);
//                fixed3 h = normalize(worldLight + viewDir);
//                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(h, worldNormal)), _Gloss);
//                #if defined (SPOT)
//                fixed atten = 1;
//                #elif defined (POINT)
//                fixed atten = 1;
//                #else
//                fixed atten = 1;
//
//                #endif
//
//                fixed3 color = ambient + (diffuse + specular) * atten;
//                return fixed4(color, 1);
//            }
//            ENDCG
//        }
    }
    Fallback "Transparent/VertLit"
}