using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace exsdk {
  public class Registery {

    // shaderInfos
    public static Dictionary<string, ShaderInfo> shaderInfos = new Dictionary<string, ShaderInfo> {
      {
        "Sprites/Default",
          new ShaderInfo() {
            type = "sprite",
            properties = new List<ShaderProperty>() {
              new ShaderProperty() { name = "_Color", type = "color", mapping = "color" },
              new ShaderProperty() { name = "_MainTex", type = "tex2d", mapping = "mainTexture" },
            }
          }
      },
      {
        "u3d-exporter/unlit",
          new ShaderInfo() {
            type = "unlit",
            properties = new List<ShaderProperty>() {
              new ShaderProperty() { name = "_Color", type = "color", mapping = "color" },
              new ShaderProperty() { name = "_MainTex", type = "tex2d", mapping = "mainTexture" },
            }
          }
      },
      {
          "u3d-exporter/phong",
          new ShaderInfo() {
            type = "phong",
            properties = new List<ShaderProperty>() {
              new ShaderProperty() { name = "_DiffuseColor", type = "color", mapping = "diffuseColor" },
              new ShaderProperty() { name = "USE_DIFFUSE_TEXTURE", type = "key", mapping = "USE_DIFFUSE_TEXTURE" },
              new ShaderProperty() { name = "_DiffuseTexture", type = "tex2d", mapping = "diffuseTexture" },
              new ShaderProperty() { name = "_SpecularColor", type = "color", mapping = "specularColor" },
              new ShaderProperty() { name = "USE_SPECULAR_TEXTURE", type = "key", mapping = "USE_SPECULAR_TEXTURE" },
              new ShaderProperty() { name = "_SpecularTexture", type = "tex2d", mapping = "specularTexture" },
              new ShaderProperty() { name = "_EmissiveColor", type = "color", mapping = "emissiveColor" },
              new ShaderProperty() { name = "USE_EMISSIVE_TEXTURE", type = "key", mapping = "USE_EMISSIVE_TEXTURE" },
              new ShaderProperty() { name = "_EmissiveTexture", type = "tex2d", mapping = "emissiveTexture" },
              new ShaderProperty() { name = "_Glossiness", type = "float", mapping = "glossiness" },
              new ShaderProperty() { name = "USE_NORMAL_TEXTURE", type = "key", mapping = "USE_NORMAL_TEXTURE" },
              new ShaderProperty() { name = "_normalTexture", type = "tex2d", mapping = "normalTexture" },
            }
          }
      },
      {
        "u3d-exporter/matcap",
          new ShaderInfo() {
            type = "matcap",
            properties = new List<ShaderProperty>() {
              new ShaderProperty() { name = "_Color", type = "color", mapping = "color" },
              new ShaderProperty() { name = "_MainTex", type = "tex2d", mapping = "mainTex" },
              new ShaderProperty() { name = "_MatcapTex", type = "tex2d", mapping = "matcapTex" },
              new ShaderProperty() { name = "_ColorFactor", type = "float", mapping = "colorFactor" },
            }
          }
      },
      {
        "u3d-exporter/pbr",
          new ShaderInfo() {
            type = "pbr",
            properties = new List<ShaderProperty>() {
              new ShaderProperty() { name = "_Albedo", type = "color", mapping = "albedo" },
              new ShaderProperty() { name = "USE_ALBEDO_TEXTURE", type = "key", mapping = "USE_ALBEDO_TEXTURE" },
              new ShaderProperty() { name = "_AlbedoTexture", type = "tex2d", mapping = "albedoTexture" },
              new ShaderProperty() { name = "_Metallic", type = "float", mapping = "metallic" },
              new ShaderProperty() { name = "USE_METALLIC_TEXTURE", type = "key", mapping = "USE_METALLIC_TEXTURE" },
              new ShaderProperty() { name = "_MetallicTexture", type = "tex2d", mapping = "metallicTexture" },
              new ShaderProperty() { name = "_Roughness", type = "float", mapping = "roughness" },
              new ShaderProperty() { name = "USE_ROUGHNESS_TEXTURE", type = "key", mapping = "USE_ROUGHNESS_TEXTURE" },
              new ShaderProperty() { name = "_RoughnessTexture", type = "tex2d", mapping = "roughnessTexture" },
              new ShaderProperty() { name = "_AO", type = "float", mapping = "ao" },
              new ShaderProperty() { name = "USE_AO_TEXTURE", type = "key", mapping = "USE_AO_TEXTURE" },
              new ShaderProperty() { name = "_AOTexture", type = "tex2d", mapping = "aoTexture" },
              new ShaderProperty() { name = "USE_NORMAL_TEXTURE", type = "key", mapping = "USE_NORMAL_TEXTURE" },
              new ShaderProperty() { name = "_normalTexture", type = "tex2d", mapping = "normalTexture" },
            }
          }
      },
      {
        "u3d-exporter/grid",
          new ShaderInfo() {
            type = "grid",
            properties = new List<ShaderProperty>() {
              new ShaderProperty() { name = "_TilingX", type = "float", mapping = "tilingX" },
              new ShaderProperty() { name = "_TilingY", type = "float", mapping = "tilingY" },
              new ShaderProperty() { name = "_BasePattern", type = "tex2d", mapping = "basePattern" },
              new ShaderProperty() { name = "_SubPattern", type = "tex2d", mapping = "subPattern" },
              new ShaderProperty() { name = "_SubPattern2", type = "tex2d", mapping = "subPattern2" },
              new ShaderProperty() { name = "_BaseColorWhite", type = "color", mapping = "baseColorWhite" },
              new ShaderProperty() { name = "_BaseColorBlack", type = "color", mapping = "baseColorBlack" },
              new ShaderProperty() { name = "_SubPatternColor", type = "color", mapping = "subPatternColor" },
              new ShaderProperty() { name = "_SubPatternColor2", type = "color", mapping = "subPatternColor2" },
              new ShaderProperty() { name = "WPOS_ON", type = "key", mapping = "useWorldPos" },
            }
          }
      },
    };

    // propertyModInfos
    public static List<ModProperty> propertyModInfos = new List<ModProperty>() {
      new ModProperty() { name = "m_Name", mapping = "name" },
      new ModProperty() { name = "m_IsActive", mapping = "_enabled", fn = val => val.ToString() == "0" ? false : true }
    };

    // componentModInfos
    public static Dictionary<string, ComponentModInfo> componentModInfos = new Dictionary<string, ComponentModInfo>() {
      {
        "UnityEngine.MeshRenderer",
        new ComponentModInfo() {
          type = "Model",
          properties = new List<ModProperty>() {
            new ModProperty() { name = "m_Enabled", mapping = "_enabled", fn = val => val.ToString() == "0" ? false : true },
            new ModProperty() { name = "m_Materials.Array.data", mapping = "materials" },
          }
        }
      },
      {
        "UnityEngine.SkinnedMeshRenderer",
        new ComponentModInfo() {
          type = "SkinningModel",
          properties = new List<ModProperty>() {
            new ModProperty() { name = "m_Enabled", mapping = "_enabled", fn = val => val.ToString() == "0" ? false : true },
          }
        }
      },
      {
        "UnityEngine.Animation",
        new ComponentModInfo() {
          type = "Animation",
          properties = new List<ModProperty>() {
            new ModProperty() { name = "m_Enabled", mapping = "_enabled", fn = val => val.ToString() == "0" ? false : true },
          }
        }
      },
      {
        "ScriptComponent",
        new ComponentModInfo() {
          type = "Script",
          properties = new List<ModProperty>() {
            new ModProperty() { name = "m_Enabled", mapping = "_enabled", fn = val => val.ToString() == "0" ? false : true },
            new ModProperty() { name="properties.Array.data", mapping="properties"},
          }
        }
      }
    };
  }
}