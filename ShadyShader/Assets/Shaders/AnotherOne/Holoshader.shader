Shader "Custom/Holoshader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_TintColor("Tink Color", Color) = (1,1,1,1)
		_Transparency("Transparent", Range(0.0, 0.9)) = 0.25
		_CutoutThresh("Cutout Threshold", Range(0.0, 1.0)) = 0.2
		_Distance("Distance", Float) = 1
		_Amplitude("Amplitude", Float) = 1
		_Speed("Speed", Float) = 1
		_Amount("Amount", Range(0.0, 1.0)) = 1
	}
	SubShader
	{
		Tags 
		{ 
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}
		
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			float _Transparency;
			float _CutoutThresh;
			float _Distance;
			float _Amplitude;
			float _Speed;
			float _Amount;


			v2f vert (appdata IN)
			{
				v2f OUT;
				IN.vertex.x += sin(_Time.y * _Speed + IN.vertex.y * _Amplitude) * _Distance * _Amount;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
				return OUT;
			}
			
			fixed4 frag (v2f IN) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, IN.uv) + _TintColor;
				col.a = 1 - sin(_Time.y * _Transparency);
				clip(col.r - _CutoutThresh);
				return col;
			}
			ENDCG
		}
	}
}
