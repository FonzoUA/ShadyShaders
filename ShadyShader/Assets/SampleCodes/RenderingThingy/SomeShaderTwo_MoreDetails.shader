Shader "Other/RenderingThingy/SomeShaderTwo_MoreDetails"
{
	Properties
	{
		_Tint ("Tint", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "black" {}
		_DetailTex ("Detail Texture", 2D) = "gray" {}
	}
	SubShader
	{
	
		Pass
		{
			CGPROGRAM

			#pragma vertex VertProg
			#pragma fragment FragProg

			#include "UnityCG.cginc"

			float4 _Tint;
			sampler2D _MainTex;
			float4 _MainTex_ST; 
			sampler2D _DetailTex;
			float4 _DetailTex_ST;
	
			struct Interpolators
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uvDetail : TEXCOORD1;
			};


			struct VertexData
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

			Interpolators VertProg (VertexData v) 
			{
				Interpolators i;
				i.position =  UnityObjectToClipPos(v.position); 
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				i.uvDetail = TRANSFORM_TEX(v.uv, _DetailTex);
				return i;
			}

		
			
			float4 FragProg (Interpolators i) : SV_TARGET0
			{
				float4 col = tex2D(_MainTex, i.uv); // Storing texel sample
				col *= tex2D(_DetailTex, i.uvDetail) * unity_ColorSpaceDouble; // unity_ColorSpaceDouble is a uniform variable that has a correct gamma correction number
				return col;
			}

			ENDCG
		}

	}
}
