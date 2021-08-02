Shader "Effects/BaseShader"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
    }
        SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #include "Assets/ShaderModel/HLSL/Core.hlsl"
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFrag
            ENDHLSL
        }
    }
}
