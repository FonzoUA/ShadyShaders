Shader "Other/RenderingThingy/SomeShaderTwo_MoreDetails"
{
	Properties
	{
		_MainTex ("Splat Map", 2D) = "black" {}
		// [NoScaleOffset] is an attribute to shader properties. In this case it will apply tiling and offset from main tex to other texs
		[NoScaleOffset] _Texture1 ("Texture 1", 2D) = "white" {} 
		[NoScaleOffset] _Texture2 ("Texture 2", 2D) = "white" {}
		[NoScaleOffset] _Texture3 ("Texture 3", 2D) = "white" {} 
		[NoScaleOffset] _Texture4 ("Texture 4", 2D) = "white" {}
	}
	SubShader
	{
	
		Pass
		{
			CGPROGRAM

			#pragma vertex VertProg
			#pragma fragment FragProg

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST; 
			// We dont need _ST's for those cause of [NoScaleOffset] attribute
			sampler2D _Texture1;
			sampler2D _Texture2;
			sampler2D _Texture3;
			sampler2D _Texture4;
	
			struct VertexData
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Interpolators
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uvSplat : TEXCOORD1; // To sample splat map
			};

			

			Interpolators VertProg (VertexData v) 
			{
				Interpolators i;
				i.position =  UnityObjectToClipPos(v.position); 
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				i.uvSplat = v.uv;
				return i;
			}

		
			
			float4 FragProg (Interpolators i) : SV_TARGET0
			{
				float4 splat = tex2D(_MainTex, i.uvSplat);
				// Since the splat map is monochrome, retrieve any color channel and use that value
				// as a if/else thing
				return tex2D(_Texture1, i.uv) * splat.r 
					+ tex2D(_Texture2, i.uv) * splat.g 
					+ tex2D(_Texture3, i.uv) * splat.b 
					+ tex2D(_Texture4, i.uv) * (1 - splat.r - splat.g - splat.b);
			}

			ENDCG
		}

	}
}
