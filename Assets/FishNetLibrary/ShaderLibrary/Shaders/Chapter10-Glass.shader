// Upgrade NOTE: replaced 'defined USING_DIRECTIONAL_LIGHT' with 'defined (USING_DIRECTIONAL_LIGHT)'

Shader"Unity Shaders Book/Chapter10-Glass"
{
    Properties
    {
        _MainTex("Main Tex",2D) = "white"{}
        _BumpMap("Normal Map",2D) = "bump"{}
        _Distortion("Distortion", Range(0,100)) = 10
        _RefractAmount("Refraction Amount",Range(0,1)) = 1
        _Cubemap("Environment Cubemap",Cube) = "_Skybox"{}
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Opaque"}
        GrabPass {"_RefractionTex"}
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
            // #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            samplerCUBE _Cubemap;
            float _Distortion;
            fixed _RefractAmount;
            sampler2D _RefractionTex;
            float4 _RefractionTex_TexelSize;

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
                fixed4 uv : TEXCOORD0;
                float4 TtoW0: TEXCOORD1;
                float4 TtoW1: TEXCOORD2;
                float4 TtoW2 : TEXCOORD3;
                float4 scrPos : TEXCOORD4;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = mul(unity_MatrixMVP,v.vertex);
                o.scrPos = ComputeGrabScreenPos(o.pos);
                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;
                
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
                float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

                fixed3 bump = UnpackNormal(tex2D(_BumpMap,i.uv.zw));

                float2 offset = bump.xy * _Distortion * _RefractionTex_TexelSize.xy;
                i.scrPos.xy = offset + i.scrPos.xy;
                fixed3 refrCol = tex2D(_RefractionTex, i.scrPos.xy/i.scrPos.w).rgb;
                bump = normalize(half3(dot(i.TtoW0.xyz,bump),dot(i.TtoW1.xyz,bump),dot(i.TtoW2.xyz,bump)));
                fixed3 reflDir = reflect(-worldViewDir,bump);
                fixed4 texColor = tex2D(_MainTex, i.uv.xy);
                fixed3 reflCol = texCUBE(_Cubemap, reflDir).rgb * texColor.rgb;

                fixed3 finalColor = reflCol * (1 - _RefractAmount) + refrCol * _RefractAmount;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    Fallback "Specular"
}
