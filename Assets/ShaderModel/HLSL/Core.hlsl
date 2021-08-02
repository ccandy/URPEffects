#ifndef EFFECT_CORE_INCLUDED
#define EFFECT_CORE_INCLUDED

#include "Assets/ShaderModel/HLSL/Common.hlsl"

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
UNITY_DEFINE_INSTANCED_PROP(float4, _BaseMap_ST)
UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)



struct VertexInput 
{
	float4 positionOS		:POSITION;
	float3 normal				:NORMAL;
	float4 tangent				:TANGENT;
	float2 baseUV				:TEXCOORD0;

	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput 
{
	float4 positionCS		:SV_POSITION;
	float2 baseUV				:TEXCOORD0;
	float3 worldNormal	:TEXCOORD1;
	float3 worldBinormal	:TEXCOORD2;

	UNITY_VERTEX_INPUT_INSTANCE_ID
};


VertexOutput LitPassVertex(VertexInput input)
{
	VertexOutput output;
	UNITY_TRANSFER_INSTANCE_ID(input, output);

	float3 positionWS = TransformObjectToWorld(input.positionOS);
	output.positionCS = TransformWorldToHClip(positionWS);

	float4 baseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseMap_ST);
	output.baseUV = input.baseUV * baseST.xy + baseST.zw;


	return output;
}

float4 LitPassFrag(VertexOutput input):SV_TARGET
{
	UNITY_SETUP_INSTANCE_ID(input);

	float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.baseUV);
	float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
	
	float4 base = baseMap * baseColor;

	return base;

}

#endif