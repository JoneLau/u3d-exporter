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
        prefab = prefab.transform.root.gameObject;
        bool isAnimPrefab = Utils.IsAnimPrefab(prefab);
        string id = Utils.AssetID(prefab);

        if (isAnimPrefab) {
          JSON_Asset jsonAsset = new JSON_Asset
          {
            type = "anim-prefab",
            urls = new Dictionary<string, string> {
              { "gltf", "skinnings/" + id + ".gltf" }
            }
          };
          result.prefab = jsonAsset;
        } else {
          JSON_Asset jsonAsset = new JSON_Asset
          {
            type = "prefab",
            urls = new Dictionary<string, string> {
              { "json", "prefabs/" + id + ".json" }
            }
          };
          result.prefab = jsonAsset;
        }
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

      // Light
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

      // Camera
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

      // Mesh
      var meshFilter = _go.GetComponent<MeshFilter>();
      if (meshFilter) {
        JSON_Component comp = new JSON_Component();
        comp.type = "Model";
        var id = Utils.AssetID(meshFilter.sharedMesh);
        JSON_Asset jsonAsset = new JSON_Asset
        {
          type = "mesh",
          urls = new Dictionary<string, string> {
            { "gltf", "meshes/" + id + ".gltf" },
            { "bin", "meshes/" + id + ".bin" }
          }
        };
        comp.properties.Add("mesh", jsonAsset);

        result.Add(comp);
      }

      return result;
    }
  }
}