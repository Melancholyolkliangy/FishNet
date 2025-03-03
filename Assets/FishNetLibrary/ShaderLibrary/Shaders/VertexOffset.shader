Shader "Custom/VertexOffset"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HeightTex ("Height Texture", 2D) = "white" {}
        _Strength ("Strength",Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
    
        Pass
        {
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 heightuv : TEXCOORD1;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _HeightTex;
            float4 _MainTex_ST;
            float4 _HeightTex_ST;
            float _Strength;
            
            v2f vert (appdata v)
            {
                v2f o;
                // 采样高度图
                float2 loopOffset = float2(_Time.y * _HeightTex_ST.z,_Time.y * _HeightTex_ST.w);
                // float2 loopOffset = _HeightTex_ST.zw;
                float height = tex2Dlod(_HeightTex, float4(v.uv * _HeightTex_ST.xy + loopOffset,1,1)).r - 0.5;
                // 根据高度值调整顶点位置
                v.vertex += float4(height * normalize(v.normal),1);
                // 转换到裁剪空间
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 采样主纹理
                float2 loopOffset = float2(_Time.y * _MainTex_ST.z,_Time.y * _MainTex_ST.w);
                fixed4 col = tex2D(_MainTex, i.uv * _MainTex_ST.xy + loopOffset);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}