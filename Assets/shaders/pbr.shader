Shader "u3d-exporter/pbr" {
  Properties {
    _Albedo ("Albedo Color", Color) = (1,1,1,1)
    [Toggle(USE_ALBEDO_TEXTURE)] _USE_ALBEDO_TEXTURE("Use Albedo Texture", Int) = 0
    _AlbedoTexture ("Albedo Texture", 2D) = "white" {}
    _Metallic ("Metallic", Float) = 1
    [Toggle(USE_METALLIC_TEXTURE)] _USE_METALLIC_TEXTURE("Use Metallic Texture", Int) = 0
    _MetallicTexture ("Metallic Texture", 2D) = "white" {}
    _Roughness ("Roughness", Float) = 0.5
    [Toggle(USE_ROUGHNESS_TEXTURE)] _USE_ROUGHNESS_TEXTURE("Use Roughness Texture", Int) = 0
    _RoughnessTexture ("Roughness Texture", 2D) = "white" {}
    _AO("Ambient Occlusion", Float) = 0.2
    [Toggle(USE_AO_TEXTURE)] _USE_AO_TEXTURE("Use AO Texture", Int) = 0
    _AOTexture("Ambient Occlusion Texture", 2D) = "white" {}
    [Toggle(USE_NORMAL_TEXTURE)] _USE_NORMAL_TEXTURE("Use Normal Texture", Int) = 0
    [Normal]_NormalTexture ("Normal Texture", 2D) = "bump" {}
  }
  SubShader{
    Tags { "RenderType" = "Opaque" }

    LOD 200
    CGPROGRAM
    // Physically based Standard lighting model, and enable shadows on all light types
    #pragma surface surf Standard fullforwardshadows nometa

    #pragma shader_feature USE_ALBEDO_TEXTURE
    #pragma shader_feature USE_METALLIC_TEXTURE
    #pragma shader_feature USE_ROUGHNESS_TEXTURE
    #pragma shader_feature USE_AO_TEXTURE
    #pragma shader_feature USE_NORMAL_TEXTURE


    // Use shader model 3.0 target, to get nicer looking lighting
    #pragma target 3.0

#if USE_ALBEDO_TEXTURE
    sampler2D _AlbedoTexture;
#endif
    fixed4 _Albedo;

#if USE_METALLIC_TEXTURE
    sampler2D _MetallicTexture;
#else
    float _Metallic;
#endif
#if USE_ROUGHNESS_TEXTURE
    sampler2D _RoughnessTexture;
#else
    float _Roughness;
#endif
#if USE_AO_TEXTURE
    sampler2D _AOTexture;
#else
    float _AO;
#endif
#if USE_NORMAL_TEXTURE
    sampler2D _NormalTexture;
#endif

    struct Input {
      float2 uv_AlbedoTexture;
      float2 uv_MetallicTexture;
      float2 uv_RoughnessTexture;
      float2 uv_AOTexture;
      float2 uv_NormalTexture;
    };

    void surf (Input IN, inout SurfaceOutputStandard o) {
#if USE_ALBEDO_TEXTURE
      fixed4 c = _Albedo * tex2D(_AlbedoTexture, IN.uv_AlbedoTexture);
#else
      fixed4 c = _Albedo;
#endif
      o.Albedo = c.rgb;
#if USE_NORMAL_TEXTURE
      o.Normal = UnpackNormal(tex2D(_NormalTexture, IN.uv_NormalTexture));
#endif
#if USE_METALLIC_TEXTURE
      o.Metallic = tex2D(_MetallicTexture, IN.uv_MetallicTexture).r;
#else
      o.Metallic = _Metallic;
#endif
#if USE_ROUGHNESS_TEXTURE
      o.Smoothness = 1.0 - tex2D(_RoughnessTexture, IN.uv_RoughnessTexture).r;
#else
      o.Smoothness = 1.0 - _Roughness;
#endif
#if USE_AO_TEXTURE
      o.Occlusion = tex2D(_AOTexture, IN.uv_AOTexture).r;
#else
      o.Occlusion = _AO;
#endif
      o.Alpha = c.a;
    }
    ENDCG
  }
  FallBack "u3d-exporter/vertex-lit"
}
