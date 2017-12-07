using UnityEngine;
using UnityEditor;

using UnityEngine.UI;

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

      // widget-component
      RectTransform rectTrans = _go.GetComponent<RectTransform>();
      if (rectTrans != null) {
        JSON_Component comp = new JSON_Component();
        comp.type = "Widget";

        comp.properties.Add("anchorLeft", rectTrans.anchorMin.x);
        comp.properties.Add("anchorRight", rectTrans.anchorMax.x);
        comp.properties.Add("anchorBottom", rectTrans.anchorMin.y);
        comp.properties.Add("anchorTop", rectTrans.anchorMax.y);

        if (rectTrans.anchorMin.x != rectTrans.anchorMax.x) {
          comp.properties.Add("marginLeft", rectTrans.offsetMin.x);
          comp.properties.Add("marginRight", -rectTrans.offsetMax.x);
          comp.properties.Add("offsetX", 0);
        } else {
          comp.properties.Add("marginLeft", 0);
          comp.properties.Add("marginRight", 0);
          comp.properties.Add("offsetX", rectTrans.anchoredPosition.x);
        }

        if (rectTrans.anchorMin.y != rectTrans.anchorMax.y) {
          comp.properties.Add("marginBottom", rectTrans.offsetMin.y);
          comp.properties.Add("marginTop", -rectTrans.offsetMax.y);
          comp.properties.Add("offsetY", 0);
        } else {
          comp.properties.Add("marginBottom", 0);
          comp.properties.Add("marginTop", 0);
          comp.properties.Add("offsetY", rectTrans.anchoredPosition.y);
        }

        comp.properties.Add("pivotX", rectTrans.pivot.x);
        comp.properties.Add("pivotY", rectTrans.pivot.y);
        comp.properties.Add("width", rectTrans.rect.width);
        comp.properties.Add("height", rectTrans.rect.height);

        result.Add(comp);
      }

      // image-component
      Image image = _go.GetComponent<Image>();
      if (image) {
        JSON_Component comp = new JSON_Component();
        comp.type = "Sprite";

        var type = "simple";
        if (image.type == Image.Type.Simple) {
          type = "simple";
        } else if (image.type == Image.Type.Sliced) {
          type = "sliced";
        } else {
          Debug.LogWarning("The image type " + image.type.ToString() + " is not supported.");
        }

        comp.properties.Add("type", type);
        comp.properties.Add("color", new float[4] {
          image.color.r,
          image.color.g,
          image.color.b,
          image.color.a
        });
        comp.properties.Add("sprite", Utils.AssetID(image.sprite));

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
      if (mods == null || mods.Length == 0) {
        return result;
      }

      // get node list from prefab
      List<GameObject> nodes = new List<GameObject>();
      Walk(new List<GameObject> { _prefab }, node => {
        nodes.Add(node);
        return true;
      });

      foreach (var mod in mods) {
        ComponentModInfo compModInfo = null;
        if (mod.target != null) {
          string typename = mod.target.GetType().ToString();
          compModInfo = Utils.GetComponentModInfo(typename);
        }

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