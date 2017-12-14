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
              new ShaderProperty() { name = "_Color", type = "color", mapping = "diffuseColor" },
              new ShaderProperty() { name = "_MainTex", type = "tex2d", mapping = "diffuse" },
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
              new ShaderProperty() { name = "_MainTex", type = "tex2d", mapping = "albedoTexture" },
              new ShaderProperty() { name = "_MetallicTex", type = "tex2d", mapping = "metallicTexture" },
              new ShaderProperty() { name = "_RoughnessTex", type = "tex2d", mapping = "roughnessTexture" },
              new ShaderProperty() { name = "_NormalTex", type = "tex2d", mapping = "normalTexture" },
              new ShaderProperty() { name = "_AOTex", type = "tex2d", mapping = "aoTexture" },
              new ShaderProperty() { name = "_OpacityTex", type = "tex2d", mapping = "opacityTexture" },
              new ShaderProperty() { name = "_EmissionTex", type = "tex2d", mapping = "emissionTexture" },
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
      }
    };
  }
}