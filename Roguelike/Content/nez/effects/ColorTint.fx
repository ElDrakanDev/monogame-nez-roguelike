sampler s0;

float4 _tintColor;

float4 mainPixel(float2 texCoord:TEXCOORD0) : COLOR0
{
	float4 pixel = tex2D(s0, texCoord);
	if (pixel.a == 0.0f)
		return pixel;
	return _tintColor;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 mainPixel();
	}
}