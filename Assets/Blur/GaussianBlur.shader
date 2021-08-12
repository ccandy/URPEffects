Shader "Effect/GaussianBlur"
{
    Properties
    {
        _BlurSize("Blur Size", Range(0,0.1)) = 0
        _Sample("Sample", float) = 1
        _Gaussian("Gaussian", float) = 0
        _StandardDeviation("StandardDeviation", float) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM

            #define PI 3.14159265359
            #define E 2.71828182846

            #pragma vertex VertexProgram
            #pragma fragment FragProgram

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_CameraColorTexture);
            SAMPLER(sampler_CameraColorTexture);

            float _BlurSize;
            float _Sample;
            float _Gaussian;
            float _StandardDeviation;

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
                float sum = 0;
                float4 col = 0;
                for (float n = 0; n < _Sample; n++) 
                {
                    float offset = (n / (_Sample - 1) - 0.5) * _BlurSize;
                    float2 uv = input.uv + float2(0, offset);

                    float stDevSquared = _StandardDeviation * _StandardDeviation;
                    float gauss = (1 / sqrt(2 * PI * stDevSquared)) * pow(E, -((offset * offset) / (2 * stDevSquared)));
                    sum += gauss;

                    col += SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uv) * gauss;
                }
                col = col / sum;
                return col;

            }


            ENDHLSL
        }
    }
}
