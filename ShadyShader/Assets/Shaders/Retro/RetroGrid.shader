// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Retro/RetroGrid"
{
	Properties
	{
		_GridThickness("Grid Thickness", Range(0, 2)) = 0.01
		_GridSpacing("Grid Spacing", float) = 10.0
		_GridColor("Grid Color", Color) = (1,1,1,1)
		_BaseColor("Base Color", Color) = (0.5, 0.5, 0.5, 1)
		_FogColor("FogColor", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags 
		{ 
			"Queue" = "Transparent"
			"RenderType"="Transparent" 
		}
		LOD 100

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			// Define vertex and pixel shader functions
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			// Include some Unity magic
			#include "UnityCG.cginc"

			// Shaderlab props
			uniform float _GridThickness;
			uniform float _GridSpacing;
			uniform fixed4 _GridColor;
			uniform fixed4 _BaseColor;
			uniform fixed4 _FogColor;
			// To vertexShader
			struct appdata
			{
				float4 vertex : POSITION;
			};

			// from vertex to pixel shader
			struct v2f
			{
				float4 Pos : SV_POSITION;
				UNITY_FOG_COORDS(1)
				float4 worldPos : TEXCOORD0;
			};

			
			// Vertex Shader
			v2f vert (appdata IN)
			{
				v2f OUT;
				OUT.Pos = UnityObjectToClipPos(IN.vertex);
				OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex);
				UNITY_TRANSFER_FOG(OUT,OUT.Pos);
				return OUT;
			}
			
			// Pixel shader 
			fixed4 frag (v2f IN) : COLOR
			{
				UNITY_APPLY_FOG(IN.fogCoord, _FogColor);
				if (frac(IN.worldPos.x / _GridSpacing) < _GridThickness || 
					frac(IN.worldPos.y / _GridSpacing) < _GridThickness)
				{
					return _GridColor;
				}
				else if (frac(IN.worldPos.x/_GridSpacing) < _GridThickness || frac(IN.worldPos.z/_GridSpacing) < _GridThickness) 
				{
					return _GridColor;
				}
				else
				{
					return _BaseColor;
				}
				
			}
			ENDCG
		}
	}
}
