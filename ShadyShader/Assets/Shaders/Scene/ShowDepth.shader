	Shader "Unlit/ShowDepth"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		//Controls whether pixels from this object are written to the depth buffer (default is On).
		//If you’re drawng solid objects, leave this on. 
		//If you’re drawing semitransparent effects, switch to ZWrite Off. 
		ZWrite On

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float depth : DEPTH;
			};

			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.depth = -UnityObjectToViewPos(v.vertex).z * _ProjectionParams.w;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float invert = 1 - i.depth;
				return fixed4(invert, invert, invert, 1) ;//* _Color;
			}
			ENDCG
		}
	}
}
