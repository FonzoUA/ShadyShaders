#if !defined(MY_LIGHTING_INCLUDED)
#define MY_LIGHTING_INCLUDED

#include "AutoLight.cginc"
#include "UnityPBSLighting.cginc"

float4 _Tint;
sampler2D _MainTex;
float4 _MainTex_ST;
float _Smoothness;
//float4 _SpecularTint;
float _Metallic;

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

UnityLight CreateLight (Interpolators i)
{
	UnityLight light;
	light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos); // _WorldSpaceLightPos0 holds the current light's POSITION
										// in case with directional light it holds direction towards the light
										// but if we are using point light, we need to compute light direction
										// by subtracting fragment's world position and normalizing result
	//float3 lightVec = _WorldSpaceLightPos0.xyz - i.worldPos; // calculate light attenuation
	//float attenuation = 1 / (1 + dot(lightVec, lightVec));
	UNITY_LIGHT_ATTENUATION(attenuation, 0, i.worldPos);
	light.color = _LightColor0.rgb * attenuation;
	light.ndotl = DotClamped(i.normal, light.dir);
	return light;
}

float4 FragProg (Interpolators i) : SV_TARGET0
{
	i.normal = normalize(i.normal);
	#if defined(POINT) || defined(SPOT)
	float3 viewDir = normalize(_WorldSpaceCameraPos -	i.worldPos); // _WorldSpaceCameraPos is a UnityShaderVariables variable that returns the position of the camera
	#else
	float3 viewDir = _WorldSpaceLightPos0.xyz; // we have directional light and thus _WorldSpaceLightPos0 is the direction
	#endif
	//float3 lightDir = _WorldSpaceLightPos0.xyz; // _WorldSpaceLightPos0 is a UnityShaderVariables variable that 
	//											// contains the position of the current light
	//float3 lightColor = _LightColor0.rgb;		// Get light source's color
	half3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;
	//albedo *= 1 - max(_SpecularTint.r, max(_SpecularTint.g, _SpecularTint.b)); // Monochrome energy concervation
	//																			// meaning we use the strongest component of the specular color to reduce the albedo
	float3 specularTint;	
	float oneMinusReflectivity;
	albedo = DiffuseAndSpecularFromMetallic (albedo, _Metallic, specularTint, oneMinusReflectivity);
	//albedo = EnergyConservationBetweenDiffuseAndSpecular(albedo, _SpecularTint.rgb, oneMinusReflectivity); // Unity's version of the energy concervation above
	//float3 diffuseL = albedo * lightColor * DotClamped(lightDir, i.normal);
	//float3 reflectionDir = reflect(-lightDir, i.normal); // reflect() takes the direction of an incoming light ray and reflects it based on a surface normal
	//float3 halfVector = normalize(lightDir + viewDir);
	//float3 specularL = specularTint * lightColor * pow(DotClamped(halfVector, i.normal), _Smoothness * 100);
	//return float4(i.normal * 0.5 + 0.5, 1); // visualize normals
	//return float4(diffuseL + specularL, 1);

	//UnityLight light; // Unity's struct to pass light data around. Cointains light's color, direction and ndotl (diffuse term);
	//light.color = lightColor;
	//light.dir = lightDir;
	//light.ndotl = DotClamped(i.normal, lightDir);

	UnityIndirect indirectLight;
	indirectLight.diffuse = 0;
	indirectLight.specular = 0;
	// Parameters: 1 - diffuse color; 2 - specular color; 3 - reflectivity; 4 - roughness; 5 - surface normal; 6 - View Direction, 7 - Direct light; 8 - Indirect Light
	//return UNITY_BRDF_PBS(albedo, specularTint, oneMinusReflectivity, _Smoothness, i.normal, viewDir, light, indirectLight );
	return UNITY_BRDF_PBS(albedo, 
		specularTint, 
		oneMinusReflectivity, 
		_Smoothness, 
		i.normal, 
		viewDir, 
		CreateLight(i), 
		indirectLight );
}

#endif