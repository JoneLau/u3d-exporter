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

    JSON_Scene DumpScene (List<GameObject> _nodes) {
      JSON_Scene scene = new JSON_Scene();

      // dump entities
      foreach ( GameObject go in _nodes ) {
        JSON_Entity ent = DumpEntity(go, _nodes);

        scene.entities.Add(ent);
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
      result.translation[0] =  _go.transform.localPosition.x;
      result.translation[1] =  _go.transform.localPosition.y;
      result.translation[2] = -_go.transform.localPosition.z;

      // rotation
      // NOTE: convert LH to RH
      result.rotation[0] = -_go.transform.localRotation.x;
      result.rotation[1] = -_go.transform.localRotation.y;
      result.rotation[2] =  _go.transform.localRotation.z;
      result.rotation[3] =  _go.transform.localRotation.w;

      // scale
      result.scale[0] = _go.transform.localScale.x;
      result.scale[1] = _go.transform.localScale.y;
      result.scale[2] = _go.transform.localScale.z;

      // children
      result.children = new List<int>();
      foreach (Transform child in _go.transform) {
        int idx = _nodes.IndexOf(child.gameObject);
        if ( idx != -1) {
          result.children.Add(idx);
        }
      }

      // check if it is a prefab
      GameObject prefab = PrefabUtility.GetPrefabParent(_go) as GameObject;
      if ( prefab ) {
        prefab = prefab.transform.root.gameObject;
        bool isAnimPrefab = Utils.IsAnimPrefab(prefab);
        string id = Utils.AssetID(prefab);
        string url = isAnimPrefab ?
          "skinnings/" + id + ".gltf" :
          "prefabs/" + id + ".json"
          ;

        result.prefab = url;
      }

      // serialize components
      result.components = DumpComponents(_go);

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
        comp.properties.Add("mesh", "meshes/" + id + ".gltf");

        result.Add(comp);
      }

      return result;
    }
  }
}