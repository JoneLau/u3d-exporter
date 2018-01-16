Shader "u3d-exporter/phong" {
  Properties {
    _DiffuseColor ("Diffuse Color", Color) = (1,1,1,1)
    [Toggle(USE_DIFFUSE_TEXTURE)] _USE_DIFFUSE_TEXTURE("Use Diffuse Texture", Int) = 0
    _DiffuseTexture ("Diffuse Texture", 2D) = "white" {}
    _SpecularColor ("Specular Color", Color) = (1,1,1,1)
    [Toggle(USE_SPECULAR_TEXTURE)] _USE_SPECULAR_TEXTURE("Use Specular Texture", Int) = 0
    _SpecularTexture ("Specular Texture", 2D) = "white" {}
    _EmissiveColor ("Emissive Color", Color) = (0,0,0,1)
    [Toggle(USE_EMISSIVE_TEXTURE)] _USE_EMISSIVE_TEXTURE("Use Emissive Texture", Int) = 0
    _EmissiveTexture ("Emissive Texture", 2D) = "black" {}
    _Glossiness ("Glossiness", Range (0, 1)) = 0.5
    [Toggle(USE_NORMAL_TEXTURE)] _USE_NORMAL_TEXTURE("Use Normal Texture", Int) = 0
    [Normal]_NormalTexture ("Normal Texture", 2D) = "bump" {}
  }

  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 200

    CGPROGRAM
      #pragma surface surf CustomPhong

      #pragma shader_feature USE_DIFFUSE_TEXTURE
      #pragma shader_feature USE_SPECULAR_TEXTURE
      #pragma shader_feature USE_EMISSIVE_TEXTURE
      #pragma shader_feature USE_NORMAL_TEXTURE

      half4 LightingCustomPhong(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
        half3 h = normalize(lightDir + viewDir);
        half ndl = max(0.0, dot(s.Normal, lightDir));
        half ndh = max(0.0, dot(s.Normal, h));
        ndh = (ndl == 0.0) ? 0.0: ndh;
        ndh = pow(ndh, max(1.0, s.Gloss * 128.0));
        half4 c;
        c.rgb = (s.Albedo * _LightColor0.rgb * ndl + _LightColor0.rgb * s.Specular * ndh) * atten + s.Emission;
        c.a = s.Alpha;
        return c;
      }
#if USE_DIFFUSE_TEXTURE
      sampler2D _DiffuseTexture;
#else
      fixed4 _DiffuseColor;
#endif
#if USE_SPECULAR_TEXTURE
      sampler2D _SpecularTexture;
#else
      fixed4 _SpecularColor;
#endif
#if USE_EMISSIVE_TEXTURE
      sampler2D _EmissiveTexture;
#else
      fixed4 _EmissiveColor;
#endif
#if USE_NORMAL_TEXTURE
      sampler2D _NormalTexture;
#endif
      float _Glossiness;

      struct Input {
        float2 uv_DiffuseTexture;
        float2 uv_SpecularTexture;
        float2 uv_EmissiveTexture;
        float2 uv_NormalTexture;
      };

      void surf (Input IN, inout SurfaceOutput o) {
#if USE_DIFFUSE_TEXTURE
        fixed4 c = tex2D(_DiffuseTexture, IN.uv_DiffuseTexture);
#else
        fixed4 c = _DiffuseColor;
#endif
        o.Albedo = c.rgb;
#if USE_SPECULAR_TEXTURE
        o.Specular = tex2D(_SpecularTexture, IN.uv_SpecularTexture).r;
#else
        o.Specular = _SpecularColor.r;
#endif
#if USE_EMISSIVE_TEXTURE
        o.Emission = tex2D(_EmissiveTexture, IN.uv_EmissiveTexture);
#else
        o.Emission = _EmissiveColor;
#endif
#if USE_NORMAL_TEXTURE
        o.Normal = UnpackNormal(tex2D(_NormalTexture, IN.uv_NormalTexture));
#endif
        o.Gloss = _Glossiness;
        o.Alpha = c.a;
      }
    ENDCG
  }

  FallBack "u3d-exporter/unlit"
}