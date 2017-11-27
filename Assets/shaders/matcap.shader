Shader "u3d-exporter/matcap" {
  Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _ColorFactor ("Color Factor", Range (0, 1)) = 0.5
    _MainTex ("Main Texture (RGB)", 2D) = "white" {}
    _MatcapTex("Matcap Texture (RGB)", 2D) = "white" {}
  }

  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 200

    CGPROGRAM
      #pragma surface surf Lambert

      sampler2D _MainTex;
      sampler2D _MatcapTex;
      fixed4 _Color;
      float _ColorFactor;

      struct Input {
        float2 uv_MainTex;
        float2 uv_MatcapTex;
      };

      void surf (Input IN, inout SurfaceOutput o) {
        fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex) * _Color;
        fixed4 matcapColor = tex2D(_MatcapTex, IN.uv_MatcapTex);
        o.Albedo = mainColor.rgb * _ColorFactor + matcapColor.rgb * (1 - _ColorFactor);
        o.Alpha = mainColor.a * _ColorFactor + matcapColor.a * (1 - _ColorFactor);
      }
    ENDCG
  }

  Fallback "u3d-exporter/vertex-lit"
}
