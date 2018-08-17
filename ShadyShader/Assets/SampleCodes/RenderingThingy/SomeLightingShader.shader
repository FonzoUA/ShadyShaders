Shader "Other/RenderingThingy/SomeLightingShader"
{
	Properties
	{
		_Tint ("Tint", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "black" {}
		//_SpecularTint ("Specular" , Color) = (0.5, 0.5, 0.5)
		[Gamma] _Metallic ("Metallic", Range(0,1)) = 0
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
	}
	SubShader
	{
	
		Pass
		{
			Tags {"LightMode" = "ForwardBase"} // Tells Unity that we are using ForwardRendering 

			CGPROGRAM

			#pragma vertex VertProg
			#pragma fragment FragProg
			// make sure we are using at least shader level 3.0
			#pragma target 3.0 

			
			#include "MultiLightShader.cginc"
			ENDCG
		}

		Pass
		{
			Tags {"LightMode" = "ForwardAdd"} // Tell Unity to use this pass to render additional light

			// How new and old pixel data is combined is defined by these two factors. The new and old data
			// is multiplied with them and then added to become final result. By default its set to "Blend One Zero"
			// aka no blending (so that old pixels are replaced by the new ones). In here we replace it with
			// "Blend One One" aka additive blending
			Blend One One

			// This dissables GPU's writing to the depth buffer. In this case we do it cause 
			// we are working with opaque objects (so no transparency issues) and hense don't 
			// need to test depth of the same surface twice (since its already done in previous pass)
			ZWrite Off

			CGPROGRAM
			#pragma vertex VertProg
			#pragma fragment FragProg
			// make sure we are using at least shader level 3.0
			#pragma target 3.0 		
			// We need to let the shader know that its dealing with point light
				#define POINT
			#include "MultiLightShader.cginc"

			ENDCG
		}

	}
}
