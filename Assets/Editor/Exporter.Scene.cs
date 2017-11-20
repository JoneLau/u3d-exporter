using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

using System.IO;

namespace exsdk {
  public partial class Exporter {

    // -----------------------------------------
    // DumpScene
    // -----------------------------------------

    JSON_Scene DumpScene(List<GameObject> _nodes) {
      JSON_Scene scene = new JSON_Scene();

      // dump entities
      int index = 0;
      foreach (GameObject go in _nodes) {
        JSON_Entity ent = DumpEntity(go, _nodes);

        if (go.transform.parent == null) {
          scene.children.Add(index);
        }
        scene.entities.Add(ent);
        index += 1;
      }

      return scene;
    }

    // -----------------------------------------
    // DumpEntity
    // -----------------------------------------

    JSON_Entity DumpEntity(GameObject _go, List<GameObject> _nodes) {
      JSON_Entity result = new JSON_Entity();

      // name
      result.name = _go.name;

      // NOTE: skinned mesh node will use identity matrix
      if (_go.GetComponent<SkinnedMeshRenderer>() != null ||
        _go.GetComponent<Canvas>() != null) {
        // translation
        result.translation[0] = 0.0f;
        result.translation[1] = 0.0f;
        result.translation[2] = 0.0f;

        // rotation
        result.rotation[0] = 0.0f;
        result.rotation[1] = 0.0f;
        result.rotation[2] = 0.0f;
        result.rotation[3] = 1.0f;

        // scale
        result.scale[0] = 1.0f;
        result.scale[1] = 1.0f;
        result.scale[2] = 1.0f;
      } else {
        // translation
        // NOTE: convert LH to RH
        result.translation[0] = _go.transform.localPosition.x;
        result.translation[1] = _go.transform.localPosition.y;
        result.translation[2] = -_go.transform.localPosition.z;

        // rotation
        // NOTE: convert LH to RH
        result.rotation[0] = -_go.transform.localRotation.x;
        result.rotation[1] = -_go.transform.localRotation.y;
        result.rotation[2] = _go.transform.localRotation.z;
        result.rotation[3] = _go.transform.localRotation.w;

        // scale
        result.scale[0] = _go.transform.localScale.x;
        result.scale[1] = _go.transform.localScale.y;
        result.scale[2] = _go.transform.localScale.z;
      }
      // children
      result.children = new List<int>();
      foreach (Transform child in _go.transform) {
        int idx = _nodes.IndexOf(child.gameObject);
        if (idx != -1) {
          result.children.Add(idx);
        }
      }

      // check if it is a prefab
      GameObject prefab = PrefabUtility.GetPrefabParent(_go) as GameObject;
      if (prefab) {
        // DELME: DO WE NEED THIS?? PLEASE CONFIRM!
        // prefab = prefab.transform.root.gameObject;
        string id = Utils.AssetID(prefab);
        result.prefab = id;

        result.modifications = DumpModifications(_go, prefab);
      } else {
        // NOTE: if we are prefab, do not serailize its components
        // serialize components
        result.components = DumpComponents(_go);
      }

      return result;
    }

    // -----------------------------------------
    // DumpComponent
    // -----------------------------------------

    List<JSON_Component> DumpComponents(GameObject _go) {
      List<JSON_Component> result = new List<JSON_Component>();

      // light-component
      var light = _go.GetComponent<Light>();
      if (light) {
        JSON_Component comp = new JSON_Component();
        comp.type = "Light";
        comp.properties.Add("color", new float[3] {
          light.color.r,
          light.color.g,
          light.color.b
        });

        result.Add(comp);
      }

      // camera-component
      var camera = _go.GetComponent<Camera>();
      if (camera) {
        JSON_Component comp = new JSON_Component();
        comp.type = "Camera";
        comp.properties.Add("backgroundColor", new float[3] {
          camera.backgroundColor.r,
          camera.backgroundColor.g,
          camera.backgroundColor.b
        });

        result.Add(comp);
      }

      // model-component
      var meshFilter = _go.GetComponent<MeshFilter>();
      if (meshFilter) {
        JSON_Component comp = new JSON_Component();
        comp.type = "Model";

        // mesh
        var id = Utils.AssetID(meshFilter.sharedMesh);
        comp.properties.Add("mesh", id);

        // materials
        Renderer renderer = _go.GetComponent<Renderer>();
        if (renderer) {
          List<string> matAssets = new List<string>();
          foreach (Material mat in renderer.sharedMaterials) {
            id = Utils.AssetID(mat);
            matAssets.Add(id);
          }
          comp.properties.Add("materials", matAssets);
        }

        result.Add(comp);
      }

      // skinning-model-component
      var skinnedMeshRenderer = _go.GetComponent<SkinnedMeshRenderer>();
      if (skinnedMeshRenderer) {
        JSON_Component comp = new JSON_Component();
        comp.type = "SkinningModel";

        // mesh
        var id = Utils.AssetID(skinnedMeshRenderer.sharedMesh);
        comp.properties.Add("mesh", id);

        // materials
        Renderer renderer = _go.GetComponent<Renderer>();
        if (renderer) {
          List<string> matAssets = new List<string>();
          foreach (Material mat in renderer.sharedMaterials) {
            id = Utils.AssetID(mat);
            matAssets.Add(id);
          }
          comp.properties.Add("materials", matAssets);
        }

        result.Add(comp);
      }

      // animation-component
      List<AnimationClip> clips = Utils.GetAnimationClips(_go);
      if (clips != null && clips.Count > 0) {
        JSON_Component comp = new JSON_Component();
        comp.type = "Animation";

        var ids = new List<string>();
        foreach (var clip in clips) {
          ids.Add(Utils.AssetID(clip));
        }
        comp.properties.Add("animations", ids);
        comp.properties.Add("joints", Utils.GetJointsID(_go));

        result.Add(comp);
      }

      // screen-component
      Canvas canvas = _go.GetComponent<Canvas>();
      if (canvas != null) {
        JSON_Component comp = new JSON_Component();
        comp.type = "Screen";

        result.Add(comp);
      }

      // screen-component
      RectTransform rectTrans = _go.GetComponent<RectTransform>();
      if (rectTrans != null) {
        JSON_Component comp = new JSON_Component();
        comp.type = "Widget";

        comp.properties.Add("alignLeft", true);
        comp.properties.Add("alignRight", true);
        comp.properties.Add("alignBottom", true);
        comp.properties.Add("alignTop", true);

        // 0 or 1 only calculate offset
        if (rectTrans.anchorMax.x == 0.5 && rectTrans.anchorMin.x == 0.5) {
          comp.properties["alignLeft"] = false;
          comp.properties["alignRight"] = false;
        } else {
          if (rectTrans.anchorMin.x == 0) {
            comp.properties.Add("left", rectTrans.offsetMin.x);
            comp.properties.Add("leftUnit", "px");
          } else if (rectTrans.anchorMin.x == 1) {
            comp.properties["alignLeft"] = false;
          } else {
            if (rectTrans.offsetMin.x != 0) {
              Debug.LogWarning("Please set your left value to 0 for percentage anchor value");
            }

            comp.properties.Add("left", rectTrans.anchorMin.x * 100);
            comp.properties.Add("leftUnit", "%");
          }

          if (rectTrans.anchorMax.x == 1) {
            comp.properties.Add("right", -rectTrans.offsetMax.x);
            comp.properties.Add("rightUnit", "px");
          } else if (rectTrans.anchorMax.x == 0) {
            comp.properties["alignRight"] = false;
          } else {
            if (rectTrans.offsetMax.x != 0) {
              Debug.LogWarning("Please set your right value to 0 for percentage anchor value");
            }

            comp.properties.Add("right", (1 - rectTrans.anchorMax.x) * 100);
            comp.properties.Add("rightUnit", "%");
          }
        }

        // 0 or 1 only calculate offset
        if (rectTrans.anchorMax.y == 0.5 && rectTrans.anchorMin.y == 0.5) {
          comp.properties["alignBottom"] = false;
          comp.properties["alignTop"] = false;
        } else {
          if (rectTrans.anchorMin.y == 0) {
            comp.properties.Add("bottom", rectTrans.offsetMin.y);
            comp.properties.Add("bottomUnit", "px");
          } else if (rectTrans.anchorMin.y == 1) {
            comp.properties["alignBottom"] = false;
          } else {
            if (rectTrans.offsetMin.y != 0) {
              Debug.LogWarning("Please set your bottom value to 0 for percentage anchor value");
            }

            comp.properties.Add("bottom", rectTrans.anchorMin.y * 100);
            comp.properties.Add("bottomUnit", "%");
          }

          if (rectTrans.anchorMax.y == 1) {
            comp.properties.Add("top", -rectTrans.offsetMax.y);
            comp.properties.Add("topUnit", "px");
          } else if (rectTrans.anchorMax.y == 0) {
            comp.properties["alignTop"] = false;
          } else {
            if (rectTrans.offsetMax.y != 0) {
              Debug.LogWarning("Please set your top value to 0 for percentage anchor value");
            }

            comp.properties.Add("top", (1 - rectTrans.anchorMax.y) * 100);
            comp.properties.Add("topUnit", "%");
          }
        }

        comp.properties.Add("pivotX", rectTrans.pivot.x);
        comp.properties.Add("pivotY", rectTrans.pivot.y);
        comp.properties.Add("width", rectTrans.rect.width);
        comp.properties.Add("height", rectTrans.rect.height);
        comp.properties.Add("offsetX", rectTrans.localPosition.x);
        comp.properties.Add("offsetY", rectTrans.localPosition.y);

        result.Add(comp);
      }

      return result;
    }

    // -----------------------------------------
    // DumpModifications
    // -----------------------------------------

    List<JSON_Modification> DumpModifications(GameObject _go, GameObject _prefab) {
      List<JSON_Modification> result = new List<JSON_Modification>();

      PropertyModification[] mods = PrefabUtility.GetPropertyModifications(_go);
      if (mods.Length == 0) {
        return result;
      }

      // get node list from prefab
      List<GameObject> nodes = new List<GameObject>();
      Walk(new List<GameObject> { _prefab }, node => {
        nodes.Add(node);
        return true;
      });

      foreach (var mod in mods) {
        string typename = mod.target.GetType().ToString();
        ComponentModInfo compModInfo = Utils.GetComponentModInfo(typename);

        // NOTE: we only care about the modifications we want
        if (compModInfo == null) {
          continue;
        }

        string mappedProp = compModInfo.MapProperty(mod.propertyPath);

        if ( mappedProp != null ) {
          JSON_Modification jsonMod = new JSON_Modification();

          // entity
          GameObject go = (mod.target as Component).gameObject;
          jsonMod.entity = nodes.IndexOf(go);

          // property
          jsonMod.property = compModInfo.type + "." + mappedProp;

          // value
          if (mod.objectReference) {
            jsonMod.value = Utils.AssetID(mod.objectReference);
          } else {
            jsonMod.value = mod.value;
          }

          result.Add(jsonMod);
        }
      }

      return result;
    }
  }
}