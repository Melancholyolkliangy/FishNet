Shader "Custom/JellyEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DeformAmount ("Deform Amount", Range(0, 1)) = 0.1
        _DeformSpeed ("Deform Speed", Range(0, 10)) = 1.0
        _DeformDecay ("Deform Decay", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _DeformAmount;
            float _DeformSpeed;
            float _DeformDecay;

            // 顶点动画函数
            float3 JellyDeform(float3 vertex, float3 normal, float time)
            {
                // 使用正弦函数模拟弹性形变
                float deform = sin(time * _DeformSpeed) * _DeformAmount;
                // 随时间衰减形变
                deform *= exp(-time * _DeformDecay);
                // 沿法线方向位移顶点
                return vertex + normal * deform;
            }

            v2f vert (appdata v)
            {
                v2f o;
                // 获取当前时间
                float time = _Time.y;
                // 应用果冻形变
                float4 deformedVertex = float4(JellyDeform(v.vertex, v.normal, time),1);
                o.vertex = UnityObjectToClipPos(deformedVertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 采样纹理
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}