Shader "Effect/BlurShader"
{
    Properties
    {
        _BlurSize("Blur Size", Range(0,0.1)) = 0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
        }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex VertexProgram
            #pragma fragment FragProgram

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_CameraColorTexture);
            SAMPLER(sampler_CameraColorTexture);

            float _BlurSize;

            struct VertexInput
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            VertexOutput VertexProgram(VertexInput input) 
            {
                VertexOutput output;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.vertex = vertexInput.positionCS;
                output.uv = input.uv;

                return output;
            }

            float4 FragProgram(VertexOutput input) : SV_Target
            {
                float2 uv = input.uv;
                float4 col = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uv);
                float4 finalCol = 0;
                for (float n = 0; n < 10; n++) 
                {
                    float2 uv = input.uv + float2(0, (n / 9 - 0.5) * _BlurSize);
                    finalCol += SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uv);
                }

                finalCol /= 10;
                return finalCol;
            }


            ENDHLSL
        }
    }
}
