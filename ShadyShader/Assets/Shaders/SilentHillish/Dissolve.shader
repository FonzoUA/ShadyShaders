Shader "Custom/Dissolve"
{
	Properties
	{
		_MainTex ("Base Texture", 2D) = "white" {}
		_AltTex ("Alternative Texture", 2D) = "white" {}
		_NoiseTex ("Noise Texture", 2D) = "white" {}
		_CutoutThresh("CutOut Threshold", Range(0.0 , 1.0)) = 0.25
		_DebugNum("DebugNum", Range(0.0, 1.0)) = 0.0
		
	}
	SubShader
	{
		Tags 
		{ 
			"Queue" = "Transparent"
			"RenderType"="Transparent" 
		}

		Pass
		{
			//Cull Off
			//ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha



			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _AltTex;
			sampler2D _NoiseTex;

			float4 _MainTex_ST;
			float _CutoutThresh;
			//uniform float _CutoutThresh;
			float _DebugNum;

			
			v2f vert (appdata IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
				UNITY_TRANSFER_FOG(OUT,OUT.vertex);
				return OUT;
			}
			
			fixed4 frag (v2f IN) : SV_Target
			{
				float cutOut = tex2D(_NoiseTex, IN.uv).r;
				fixed4 altTex = tex2D(_AltTex, IN.uv);
				fixed4 mainTex = tex2D(_MainTex, IN.uv);

				fixed4 col = mainTex;

				//col = lerp(mainTex, altTex, _DebugNum + cutOut);
				if (cutOut < _CutoutThresh)
				{
					col = altTex;
					clip(col.r - _DebugNum);
				}
				


				// Ensure transparency
				//clip(col.g - _CutoutThresh);
				// apply fog
				UNITY_APPLY_FOG(IN.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
