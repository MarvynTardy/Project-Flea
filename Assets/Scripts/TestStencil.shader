Shader "Custom/TestStencil"
{
    Properties{}

	SubShader
	{

		Tags 
		{ 
			"RenderType" = "Opaque" 
		}
 
		Pass
		{
			ZWrite Off
		}
	}
}
