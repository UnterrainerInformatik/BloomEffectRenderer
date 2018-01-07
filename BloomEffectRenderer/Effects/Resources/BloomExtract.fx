Texture2D Texture;
SamplerState TextureSampler
{
    Texture = <Texture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

float BloomThreshold;

float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float4 c = Texture.Sample(TextureSampler, texCoord);

    return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}


technique BloomExtract
{
    pass Pass1
    {
        #if SM4
			PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
		#elif SM3
			PixelShader = compile ps_3_0 PixelShaderFunction();
		#else
			PixelShader = compile ps_2_0 PixelShaderFunction();
		#endif
    }
}
