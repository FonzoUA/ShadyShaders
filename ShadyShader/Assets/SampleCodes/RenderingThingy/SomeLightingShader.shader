Shader "Other/RenderingThingy/SomeLightingShader"
{
	Properties
	{
		_Tint ("Tint", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "black" {}
		_SpecularTint ("Specular" , Color) = (0.5, 0.5, 0.5)
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

			#include "UnityStandardBRDF.cginc"
			#include "UnityStandardUtils.cginc"

			float4 _Tint;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Smoothness;
			float4 _SpecularTint;

			struct VertexData
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL; // grab normals data from verts
			};

			struct Interpolators
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2; 
			};


			

			Interpolators VertProg (VertexData v) 
			{
				Interpolators i;
				i.position =  UnityObjectToClipPos(v.position);
				i.worldPos = mul(unity_ObjectToWorld, v.position); // Determine the world position of the surface 
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				i.normal = UnityObjectToWorldNormal(v.normal);
				return i;
			}

		
			
			float4 FragProg (Interpolators i) : SV_TARGET0
			{
				i.normal = normalize(i.normal);
				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos); // _WorldSpaceCameraPos is a UnityShaderVariables variable that returns the position of the camera
				float3 lightDir = _WorldSpaceLightPos0.xyz; // _WorldSpaceLightPos0 is a UnityShaderVariables variable that 
															// contains the position of the current light
				float3 lightColor = _LightColor0.rgb;		// Get light source's color
				float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;
				//albedo *= 1 - max(_SpecularTint.r, max(_SpecularTint.g, _SpecularTint.b)); // Monochrome energy concervation
				//																			// meaning we use the strongest component of the specular color to reduce the albedo
				float oneMinusReflectivity; 
				albedo = EnergyConservationBetweenDiffuseAndSpecular(albedo, _SpecularTint.rgb, oneMinusReflectivity); // Unity's version of the energy concervation above
				float3 diffuseL = albedo * lightColor * DotClamped(lightDir, i.normal);
				//float3 reflectionDir = reflect(-lightDir, i.normal); // reflect() takes the direction of an incoming light ray and reflects it based on a surface normal
				float3 halfVector = normalize(lightDir + viewDir);
				float3 specularL = _SpecularTint.rgb * lightColor * pow(DotClamped(halfVector, i.normal), _Smoothness * 100);
				//return float4(i.normal * 0.5 + 0.5, 1); // visualize normals
				return float4(diffuseL + specularL, 1);

			}

			// The color of the diffuse reflectivity of a material is known as its albedo. Albedo is Latin for whiteness. 
			// So it describes how much of the red, green, and blue color channels are diffusely reflected. 

			// Besides diffuse reflections, there are also specular reflections. This happens when light doesn't 
			// get diffused after hitting a surface. Instead, the light ray bounces off the surface at and angle 
			// equal to the angle at which it hit the surface aka mirror. 

			// The size of the highlight produced by this effect depends on the roughness of the material. 
			// Smooth materials focus the light better, so they have smaller highlights.

			ENDCG
		}

	}
}
