Shader "Other/RenderingThingy/SomeShader"
{
	// Configurable shader parameters
	Properties
	{
		// Name ("string", type) = default value
		_Tint ("Tint", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "black" {}
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
			sampler2D _MainTex;
			float4 _MainTex_ST; // _ST stands for Scale and Transformation. Each texture within Unity stores the
								// extra texture data for tiling and offset controls. We access those via <name>_ST suffix

			// to reduce the clusterfuck that this thing is about to become (or maybe already is) 
			// and because output of the VertProg has to match input of the FragProg
			// store all that stuff in the struct
			struct Interpolators
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			// Input of the VertProg
			struct VertexData
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0; // take in the UV data of the mesh
			};

			// define the programs specified
			// VertProg has to return finals coords of a vertex 
			// since we are using 4x4 Transform matrices return float4
			// This is a semantic				[  |  ]
			// it describes the	data we trying  [  |  ] to represent to GPU
			// in this case, SV is  for "system [ \|/ ] value" and POSITION is for the final vertex position
			// old code: float4 VertProg(stuff) : SV_POSITION
			Interpolators VertProg (VertexData v) 
			{
				Interpolators i;
				i.position =  UnityObjectToClipPos(v.position); // multiply raw vert position with viewProj matrix 
				i.uv = TRANSFORM_TEX(v.uv, _MainTex); // TRANSFORM_TEX is a UnityCG.cginc macro that does the texture offset/tiling process simpler
													// definition: #define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)
				//i.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw; // Assign offset and tiling
				return i;
			}

			// FragProg suppose to return color of one pixel hense float4
			// SV_TARGET# semantic represents default shader target aka frame buffer that has the image we are generating
			// since output of the vertex program is used as input for the fragment program, it should get the parameter
			// that matches the vertex program's output
			float4 FragProg (Interpolators i) : SV_TARGET0
			{
				// return float4(i.uv, 1, 1); // Show mesh's UV data where U becomes red and V becomes green
				return tex2D(_MainTex, i.uv); // tex2D sample's the texture with the UV coords
			}

			// Unity's shader compiler takes this code and transforms into a different program depending on target platform
			// such as OpenGL for Mac, OpenGL ES for mobile, etc so we are dealing with bunch of compilers here

			// >WRAP MODE: (gonna stick this here) When importing textures in Unity, WrapMode dictates what happens when sapling UV coords
			// that are outside of 0-1 range. If WrapMode = Clamp, the UV are constrained within 0-1 range aka pixels beyond the 
			// edge would be the same as those that lie on the edge. If WrapMode = Repeat, it repeats (duuuuh) aka pixels beyond
			// the edge would be the same as those on the opposite side of the texture

			// >FILTERING: Filter Mode takes care of the situations when texels don't exactly match the pixels they are projected onto
			// Point filter (aka no filter) uses nearest texel when texture is sampled. Give the texture blocky appearance unless
			// texels map exactly to display pixels (good for pixel-perfect rendering or if you want blocky style)
			// Bilinear filter (aka default boyo) samples between two textels which are then interpolated. Does it for both U and V axises
			// hence the bilinear filterig vs linear filtering. This approach is good when texel density is less than display pixel density
			// aka zooming in to the texture which would cause a blurry look. If zoomed out of the texture, adjacent display pixels will sample
			// more than one texel apart meaning parts of the texture would be skipped meaning harsher transitions meaning sharper image.
			// This could be fixed by using mip maps
			// Trilinear fileter is similar to bilinear but also interpolates between adjasent mipmap levels
			// Theres also anisotrofic filtering: it decouples dimentions and uniformly scales down the texture with diferent versions
			// scaled for different amount in either dimention, producing better pictures (ex. mipmaps scaled to be 256x128 or 256x64 instead of 256x256 square)
			// anisotrofic filtering's extra mipmaps arent pre-generated so its bit more computationally expensive

			ENDCG
		}

	}
}
