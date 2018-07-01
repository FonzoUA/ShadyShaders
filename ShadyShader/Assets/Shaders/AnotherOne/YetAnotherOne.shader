Shader "Custom/YetAnotherOne"
{
	Properties
	{
		_MainTexture("Main Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
	}

	SubShader
	{

		Pass
		{
			CGPROGRAM

			#pragma vertex vertexFunction
			#pragma fragment fragmentFunction

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

			float4 _Color;
			sampler2D _MainTexture;

			v2f vertexFunction(appdata IN)
			{
				v2f OUT;

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;

				return OUT;
			}


			fixed4 fragmentFunction(v2f IN) : SV_Target
			{
				fixed4 col = tex2D(_MainTexture, IN.uv + float2(0, sin(IN.vertex.x*0.05f + _Time[1]*0.5f) * 0.001f));
				return col;
			}


			ENDCG
		
		}

	}
}
