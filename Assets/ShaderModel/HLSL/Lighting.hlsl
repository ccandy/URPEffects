#ifndef EFFECT_LIGHTING_INCLUDED
#define EFFECT_LIGHTING_INCLUDED

//Hair Lighting

TEXTURE2D(_ShiftMap);
SAMPLER(sampler_ShiftMap);

TEXTURE2D(_NoiseTex);
SAMPLER(sampler_NoiseTex);

float primaryShift;
float secondaryShift;

float specExp1;
float specExp2;

float4 specColor1;
float4 specColor2;

float3 ShiftTangent(float3 T, float3 N, float shift)
{
	float3 shiftT = T + N * shift;
	return normalize(shiftT);
}

float3 StrandSpecular(float3 T, float3 V, float3 L, float exp) 
{
	float3 H = normalize(L + V);
	float dotTH = dot(T, H);
	float sinTH = sqrt(1.0 - dotTH * dotTH);
	float dirAtten = smoothstep(-1.0, 0.0, dot(T, H));

	return dirAtten * pow(sinTH, exp);
}

float4 HairLighting(float3 T, float3 N, float3 lightVec, float3 viewVec, float2 uv, float ambOcc) 
{
	//diffuse
	float3 diffuse = saturate(lerp(0.25, 1, dot(N, lightVec)));

	//Sample shift tex
	float shiftTex = SAMPLE_TEXTURE2D(_ShiftMap, sampler_ShiftMap, uv);
	shiftTex -= 0.5;

	float3 t1 = ShiftTangent(T, N, primaryShift + shiftTex);
	float3 t2 = ShiftTangent(T, N, primaryShift + shiftTex);

	float3 spec1 = specColor1 * StrandSpecular(t1, viewVec, lightVec, specExp1);

	//spec2 use noise mask
	float nosie = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, uv);
	float3 spec2 = nosie * specColor2 * StrandSpecular(t2, viewVec, lightVec, specExp2);
	
	float4 o;
	o.rgb = (diffuse + spec1 + spec2) * ambOcc;
	o.a = 1;

	return o;

}





#endif