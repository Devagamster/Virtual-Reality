sampler mySampler;

float factor;
float xCenter;
float yCenter;

float4 OculusPixelShaderFunction(float4 pos: POSITION, float4 col : COLOR, float2 coords : TEXCOORD0) : COLOR0
{
	const float2 warpCenter = float2(xCenter, yCenter);
	float2 centeredTexcoord = coords - warpCenter;
	float2 warped = normalize(centeredTexcoord);
	float rescaled = tan(length(centeredTexcoord) * factor) / tan(0.5 * factor);
	warped *= 0.5 * rescaled;
	warped += warpCenter;

	float4 result;
	if (warped.x > 0 && warped.x < 1 && warped.y > 0 && warped.y < 1)
	{
		result = tex2D(mySampler, warped);
	}
	else
	{
		result = float4(0.0, 0.0, 0.0, 1.0);
	}

	return result;
}

technique Technique1  
{
	pass P0
	{
		PixelShader = compile ps_4_0_level_9_3 OculusPixelShaderFunction();
	}
}
