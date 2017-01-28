Shader "Custom/BasicGradient"
{
	Properties
	{
	}

	SubShader
	{
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct vertexIn {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(vertexIn input)
			{
				v2f output;

				output.pos = mul(UNITY_MATRIX_MVP, input.pos);
				output.uv = input.uv;

				return output;
			}
			uniform float dx;
			uniform float dy;
			uniform int SizeX;
			uniform int SizeY;
			uniform int _Points_Length;
			uniform fixed4 _Points[24];
			//sampler2D _RampTex;

			fixed4 frag(v2f input) : COLOR
			{
				int xindex = input.uv.x / dx;
				int yindex = input.uv.y / dy;
				float xvalue = input.uv.x % dx;
				float yvalue = input.uv.y % dy;

				int colum = yindex * SizeX;
				float3 colx = lerp(_Points[colum + xindex], _Points[colum + xindex + 1], xvalue/ dx);
				float3 colxx = lerp(_Points[colum + xindex + SizeX], _Points[colum + xindex + 1+ SizeX], xvalue / dx);
				float3 col = lerp(colx, colxx, yvalue / dy);
				return fixed4(col.r,col.g,col.b,1);
				//return fixed4(colx.r, colx.g, colx.b, 1);

				//return fixed4(_Points[0].r,_Points[0].g,_Points[0].b,1);
				//return lerp(_x1y1, _x2y1, input.uv.y);
				//return lerp(_BottomColor, _TopColor, tex2D(_RampTex, input.uv).a);
			}
			ENDCG
		}
	}
}