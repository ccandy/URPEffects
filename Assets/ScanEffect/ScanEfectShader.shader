Shader "Effects/ScanEfectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanVelocity("Scan Velocity", float) = 1
        _ScanDistance("Scan Distance", float) = 0
        _ScanWidth("Scan Distance", float) = 10
        _ScanColor("Scan Color", color) = (1,1,1,1)
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
                
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _CameraDepthTexture;
            float4 _MainTex_ST;

            float4 _ScanColor;

            float _ScanVelocity, _ScanDistance,_ScanWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture

                float depth = tex2D(_CameraDepthTexture, i.uv);
                float linear01Depth = Linear01Depth(depth);

                fixed4 col = tex2D(_MainTex, i.uv);

                if (linear01Depth < _ScanDistance && linear01Depth > _ScanDistance - _ScanWidth && linear01Depth < 1) 
                {
                    float diff = 1 - (_ScanDistance - linear01Depth) / _ScanWidth;
                    _ScanColor *= diff;

                    col += _ScanColor;
                    
                }
                return col;
                
                
                return col;
            }
            ENDCG
        }
    }
}
