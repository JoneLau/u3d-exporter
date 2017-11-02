Shader "u3d-exporter/pbr" {
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}
    _MetallicTex ("Metallic (RGB)", 2D) = "white" {}
    _RoughnessTex ("Roughness (RGB)", 2D) = "white" {}
    _NormalTex ("Normal (RGB)", 2D) = "white" {}
    _AOTex ("Ambient Occlusion (RGB)", 2D) = "white" {}
    _OpacityTex ("Opacity (RGB)", 2D) = "white" {}
    _EmissionTex ("Emission (RGB)", 2D) = "black" {}
  }
  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 200
    CGPROGRAM
    // Physically based Standard lighting model, and enable shadows on all light types
    #pragma surface surf Standard noshadow finalcolor:mycolor

    // Use shader model 3.0 target, to get nicer looking lighting
    #pragma target 3.0

    sampler2D _MainTex;
    sampler2D _MetallicTex;
    sampler2D _RoughnessTex;
    sampler2D _NormalTex;
    sampler2D _AOTex;
    sampler2D _OpacityTex;
    sampler2D _EmissionTex;

    struct Input {
      float2 uv_MainTex;
      float2 uv_MetallicTex;
      float2 uv_RoughnessTex;
      float2 uv_NormalTex;
      float2 uv_AOTex;
      float2 uv_OpacityTex;
      float2 uv_EmissionTex;
    };

    fixed4 _Color;

    // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
    // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
    // #pragma instancing_options assumeuniformscaling
    UNITY_INSTANCING_CBUFFER_START(Props)
    // put more per-instance properties here
    UNITY_INSTANCING_CBUFFER_END

    void mycolor (Input IN, SurfaceOutputStandard o, inout fixed4 color) {
      color = color / (color + 1.0);
    }

    void surf (Input IN, inout SurfaceOutputStandard o) {
      // Albedo comes from a texture tinted by color
      fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
      o.Albedo = c.rgb;
      o.Normal = tex2D (_NormalTex, IN.uv_NormalTex);
      o.Metallic = tex2D (_MetallicTex, IN.uv_MetallicTex).r;
      o.Smoothness = 1.0 - tex2D (_RoughnessTex, IN.uv_RoughnessTex).r;
      o.Occlusion = tex2D (_AOTex, IN.uv_AOTex).r;
      o.Emission = tex2D (_EmissionTex, IN.uv_EmissionTex).rgb;
      o.Alpha = c.a * tex2D (_OpacityTex, IN.uv_OpacityTex).r;
    }
    ENDCG
  }
  FallBack "u3d-exporter/vertex-lite"
}
