Shader "u3d-exporter/grid" {
	Properties {
		_TilingX ("Global Tiling X", Float) = 1
		_TilingY ("Global Tiling Y", Float) = 1

		_PatternColorA1 ("Pattern Color A1", Color) = (0.8,0.8,0.8,1)
		_PatternColorA2 ("Pattern Color A2", Color) = (0.8,0.8,0.8,1)
		_PatternTexA ("Patter Texture A", 2D) = "black" {}

		_PatternColorB ("Pattern Color B", Color) = (0.7,0.7,0.7,1)
		_PatternTexB ("Pattern Texture B", 2D) = "black" {}

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		#pragma target 4.0

		fixed4 _BaseColor;

		fixed4 _PatternColorA1;
		fixed4 _PatternColorA2;
		sampler2D _PatternTexA;

		fixed4 _PatternColorB;
		sampler2D _PatternTexB;

		struct Input {
			float2 uv_PatternTexA;
			float2 uv_PatternTexB;

			float3 worldNormal;
			float3 worldPos;
		};

		half _TilingX;
		half _TilingY;

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float2 overT = float2(_TilingX,_TilingY);

			float2 UV_A = IN.uv_PatternTexA * overT;
			float2 UV_B = IN.uv_PatternTexB * overT;

			fixed4 texColA = tex2D (_PatternTexA, UV_A);
			fixed4 texColB = tex2D (_PatternTexB, UV_B);

			fixed4 colBase = (_PatternColorA1 * texColA + _PatternColorA2 * (1 - texColA));
      fixed4 colFinal =
        colBase * (1 - texColB) +
        (_PatternColorB * _PatternColorB.a + colBase * (1-_PatternColorB.a)) * texColB
        ;

      // METHOD2:
			// fixed4 colBase = (_PatternColorA1 * texColA + _PatternColorA2 * (1 - texColA));
			// fixed4 colPattern = texColA * (1 - texColB) + (1 - texColA) * texColB;
      // fixed4 colFinal = colBase * (1 - colPattern) +
      //   (_PatternColorB * _PatternColorB.a + colBase * (1-_PatternColorB.a)) * colPattern
      //   ;

			o.Albedo = colFinal.rgb;
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
