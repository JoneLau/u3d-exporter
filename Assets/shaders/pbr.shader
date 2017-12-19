Shader "u3d-exporter/pbr" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}
    _MetallicTex ("Metallic (RGB)", 2D) = "white" {}
    _RoughnessTex ("Roughness (RGB)", 2D) = "white" {}
    [Normal]_NormalTex ("Normal (RGB)", 2D) = "bump" {}
    _AOTex ("Ambient Occlusion (RGB)", 2D) = "white" {}
  }
  SubShader {
    Tags { "RenderType"="Opaque" }

    LOD 200
    CGPROGRAM
    // Physically based Standard lighting model, and enable shadows on all light types
    #pragma surface surf Standard fullforwardshadows nometa

    // Use shader model 3.0 target, to get nicer looking lighting
    #pragma target 3.0

    sampler2D _MainTex;
    sampler2D _MetallicTex;
    sampler2D _RoughnessTex;
    sampler2D _NormalTex;
    sampler2D _AOTex;

    struct Input {
      float2 uv_MainTex;
    };

    fixed4 _Color;

    void surf (Input IN, inout SurfaceOutputStandard o) {
      // Albedo comes from a texture tinted by color
      fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
      o.Albedo = c.rgb;
      o.Normal = UnpackNormal(tex2D (_NormalTex, IN.uv_MainTex));
      o.Metallic = tex2D (_MetallicTex, IN.uv_MainTex).r;
      o.Smoothness = 0.5;//tex2D (_RoughnessTex, IN.uv_RoughnessTex).r;
      o.Occlusion = tex2D (_AOTex, IN.uv_MainTex).r;
      o.Alpha = c.a;
    }
    ENDCG
  }
  FallBack "u3d-exporter/vertex-lit"
}
