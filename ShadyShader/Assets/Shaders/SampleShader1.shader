Shader "Unlit/SampleShader1"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SecondTex("Texture Second", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Tween("Tween", Range(0, 1)) = 0
	}
	SubShader
	{
		Tags 
		{ 
			"Queue" = "Transparent" 
		}

		Pass
		{
			//Blend SrcAlpha OneMinusSrcAlpha
			Blend One One

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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _SecondTex;
			float4 _Color;
			float4 _MainTex_ST;
			float _Tween;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float4 color1 = tex2D(_MainTex, i.uv * 2);
				float4 color2 = tex2D(_SecondTex, i.uv);
				float4 col = lerp(color1, color2, _Tween) * float4(0.9, 0.6, i.uv.g, 1);
				return col;
			}
			ENDCG
		}
	}
}
