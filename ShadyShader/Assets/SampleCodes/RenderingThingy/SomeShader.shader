Shader "Other/RenderingThingy/SomeShader"
{
	// Configurable shader parameters
	Properties
	{
		// Name ("string", type) = default value
		_Tint ("Tint", Color) = (1,1,1,1)

	}
	// Subshaders group multiple shader variants together
	// allows to provide different sub-shaders for different builds / levels / details
	// > must contain at least one pass
	// ex. desktop subshader / mobile SubShader
	SubShader
	{
		// Shader pas is where object actually gets rendered
		// > possible to have more than one (can be rendered multiple times per frame)
		Pass
		{
			// Shader program (in Unity's shading language which on turn is a combination of HLSL and CG)
			// CGPROGRAM indicates start of the code block
			// ENDCG indicated - you wont believe - the end of the code block 
			CGPROGRAM

			// Shader must have vertex and fragment programs or else it wont work
			// vertex boi processes verts data of a mesh including conversion from object to display space
			// fragment boi is coloring the pixels within mesh's triangles
			// so we tell compiler which programs to use
			#pragma vertex VertProg
			#pragma fragment FragProg

			// add some boilerplate code that defines common variables, functions, etc
			// UnityCG.cginc is default Unity package that has bunch of useful crap such as:
			// >UnityShaderVariables - defines a whole bunch of variables such as transformation, camera, light data
			// >HLSLSupport - allows to use the same code regardless of the target playform
			// >UnityInstancing - adds instancing support (has dependency with UnityShaderVariables)
			#include "UnityCG.cginc"

			// to use the property, we need to add it to the shader code
			// the name has to exactly match the property 
			float4 _Tint;

			// to reduce the clusterfuck that this thing is about to become (or maybe already is) 
			// and because output of the VertProg has to match input of the FragProg
			// store all that stuff in the struct
			struct Interpolators
			{
				float4 position : SV_POSITION;
				float3 localPosition : TEXCOORD0;
			};

			// define the programs specified
			// VertProg has to return finals coords of a vertex 
			// since we are using 4x4 Transform matrices return float4
			// This is a semantic				[  |  ]
			// it describes the	data we trying  [  |  ] to represent to GPU
			// in this case, SV is  for "system [ \|/ ] value" and POSITION is for the final vertex position
			// old code: float4 VertProg(stuff) : SV_POSITION
			Interpolators VertProg (float4 position : POSITION) 
			{
				Interpolators i;
				i.localPosition = position.xyz;
				i.position =  UnityObjectToClipPos(position); // multiply raw vert position with viewProj matrix 
				return i;
			}

			// FragProg suppose to return color of one pixel hense float4
			// SV_TARGET# semantic represents default shader target aka frame buffer that has the image we are generating
			// since output of the vertex program is used as input for the fragment program, it should get the parameter
			// that matches the vertex program's output
			float4 FragProg (Interpolators i) : SV_TARGET0
			{
				return float4(i.localPosition + 0.5, 1) * _Tint;
			}

			// Unity's shader compiler takes this code and transforms into a different program depending on target platform
			// such as OpenGL for Mac, OpenGL ES for mobile, etc so we are dealing with bunch of compilers here


			ENDCG
		}

	}
}
