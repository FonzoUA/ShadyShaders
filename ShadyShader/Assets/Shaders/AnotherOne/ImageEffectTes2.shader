Shader "Custom/ImageEffectTes2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RedFloat ("RedFloat", Range(0, 1)) = 0
		_GreenFloat ("GreenFloat", Range(0, 1)) = 0
		_BlueFloat ("BlueFloat", Range(0, 1)) = 0
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _RedFloat;
			float _GreenFloat;
			float _BlueFloat;

			fixed4 frag (v2f i) : SV_Target
			{
				//fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col = tex2D(_MainTex, i.uv);
				float3 modColor = (_RedFloat, _GreenFloat, _BlueFloat);
				col.r = col.r * modColor.r;
				col.b = col.b * modColor.b;
				col.g = col.g * modColor.g;

				return col;
			}
			ENDCG
		}
	}
}
