Shader "Effect/AniShader"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white" {}
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
