Shader "Custom/AnotherSampleShader"
{
	Properties // aka public variables for shader material 
	{
		//variable  |public name   |type
		_MainTexture("Main Color", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)

		_DissolveTexture("Dissolve Texture", 2D) = "white" {}
		_DissolveAmount("Dissolve amt", Range(-1, 1)) = 1

		_ExtrudeAmount("Extrude Amount", Range(-5, 5)) = 1
	}

	SubShader // can have multiple for different properties
	{
		Pass // take data and draw it to the screen  (can have multiple as well in order)
		{
			CGPROGRAM // Open CG (start writing 'C' for graphics)
			
			#pragma vertex vertexFunction
			#pragma fragment fragmentFunction

			#include "UnityCG.cginc"

			/* 
				Sort of how the pipeline works 
				_____________________						_____________________
				|	Object Data		|				 ______|	Custom Data		|
				---------------------				 |	   ---------------------
						 |		_____________________|					|
						 V		|										V
				________________V______________________________________________________
		Shader	|  "Vertex function"   -> "Vertex to fragment" -> "Fragment Function" |
				-----------------------------------------------------------------------
																			|
																			V
																	_____________________
																	|	Object Data		|
																	---------------------
			*/


			// What are we going to get aka
			// Vertices
			// Normals
			// Color
			// UV
			// etc
			struct appdata 
			{
				float4 vert : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f 
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			// Reimport variables into CGPROGRAM
			float4 _Color;
			sampler2D _MainTexture;

			sampler2D _DissolveTexture;
			float _DissolveAmount;
			
			float _ExtrudeAmount;
			// v2f = vertex to fragment
			// Vertex
			// Build object
			v2f vertexFunction (appdata IN)
			{
				v2f OUT;
				
				IN.vert.xyz += IN.normal.xyz * _ExtrudeAmount * sin(_Time.y);
								// Model View Projection
				OUT.position = UnityObjectToClipPos(IN.vert);
				OUT.uv = IN.uv;
				
				return OUT;
			};

			// Fragment
			// Color it in					// Target to the screen or it breaks
			fixed4 fragmentFunction(v2f IN) : SV_Target
			{
				float4 textureColor = tex2D(_MainTexture, IN.uv);
				float4 dissolveColor = tex2D(_DissolveTexture, IN.uv);

				return textureColor - (dissolveColor * sin(_Time.y) * _Color);
			};


			ENDCG // Close CG (stop writing 'C' for graphic)
		}
	}
}