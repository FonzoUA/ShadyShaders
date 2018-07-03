Shader "Custom/PixelatedImageEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_PixColum("Pixel Colums", Float) = 64
		_PixRow("Pixel Rows", Float) = 64

	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

			v2f vert (appdata IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;
				return OUT;
			}
			
			sampler2D _MainTex;
			float _PixColum;
			float _PixRow;

			fixed4 frag (v2f IN) : SV_Target
			{
				// Splits the screen into rows and cols
				// then convers images ui to match 
				// the values passed in
				float2 uv = IN.uv;

				uv.x = round(uv.x * _PixColum) /  _PixColum;
				uv.y = round(uv.y * _PixRow) / _PixRow;

				fixed4 col = tex2D(_MainTex, uv);
				
				return col;
			}
			ENDCG
		}
	}
}
