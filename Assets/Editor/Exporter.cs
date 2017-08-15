using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;
using UnityEditor.SceneManagement;

using System.Collections;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace exsdk {
  public enum FileMode {
    Text, // all in one json file
    Mixed, // json file + bin file(s)
    Binary, // packed json + bin in one file
  }

  public delegate bool WalkCallback ( GameObject _go );

  public partial class Exporter {
    public string outputPath;
    public string name;
    public FileMode mode;

    public void Exec () {
      if ( !Directory.Exists(this.outputPath) ) {
        Debug.LogError("u3d-exporter Failed: Can not find the path \"" + this.outputPath + "\"");
        return;
      }

      // create dest folder
      string name = string.IsNullOrEmpty(this.name) ? "exports" : this.name;
      string dest = Path.Combine(this.outputPath, name);

      if ( Directory.Exists(dest) ) {
        System.IO.DirectoryInfo di = new DirectoryInfo(dest);
        di.Delete(true);
      }

      // get root objects in scene
      Scene scene = EditorSceneManager.GetActiveScene();

      // get data from scene
      var nodes = new List<GameObject>();
      var prefabs = new List<GameObject>();
      var meshes = new List<Mesh>();
      var animPrefabs = new List<GameObject>();

      WalkScene(scene, nodes, prefabs, meshes, animPrefabs);

      // save meshes
      var destMeshes = Path.Combine(dest, "meshes");
      foreach ( Mesh mesh in meshes ) {
        GLTF gltf = new GLTF();
        gltf.asset = new GLTF_Asset {
          version = "1.0.0",
          generator = "u3d-exporter"
        };
        BufferInfo bufInfo = new BufferInfo {
          id = Utils.AssetID(mesh),
          name = mesh.name
        };

        DumpMesh(mesh, gltf, bufInfo, 0);
        DumpBuffer(bufInfo, gltf);

        Save(
          destMeshes,
          Utils.AssetID(mesh),
          gltf,
          new List<BufferInfo> {bufInfo}
        );
      }

      // save skins
      var destSkins = Path.Combine(dest, "skinnings");
      foreach ( GameObject animPrefab in animPrefabs ) {
        GLTF gltf = new GLTF();
        gltf.asset = new GLTF_Asset {
          version = "1.0.0",
          generator = "u3d-exporter"
        };
        BufferInfo bufInfo = new BufferInfo {
          id = Utils.AssetID(animPrefab),
          name = animPrefab.name
        };

        DumpSkin(animPrefab, gltf, bufInfo);
        DumpBuffer(bufInfo, gltf);

        Save(
          destSkins,
          Utils.AssetID(animPrefab),
          gltf,
          new List<BufferInfo> {bufInfo}
        );
      }

      // save animations
      var destAnims = Path.Combine(dest, "anims");
      foreach ( GameObject animPrefab in animPrefabs ) {
        GLTF gltf = new GLTF();
        gltf.asset = new GLTF_Asset {
          version = "1.0.0",
          generator = "u3d-exporter"
        };
        BufferInfo bufInfo = new BufferInfo {
          id = Utils.AssetID(animPrefab),
          name = animPrefab.name
        };

        DumpAnims(animPrefab, gltf, bufInfo);
        DumpBuffer(bufInfo, gltf);

        Save(
          destAnims,
          Utils.AssetID(animPrefab),
          gltf,
          new List<BufferInfo> {bufInfo}
        );
      }

      // save prefabs
      var destPrefabs = Path.Combine(dest, "prefabs");
      // create dest directory
      if ( !Directory.Exists(destPrefabs) ) {
        Directory.CreateDirectory(destPrefabs);
      }
      foreach ( GameObject prefab in prefabs ) {
        var prefabJson = DumpPrefab(prefab);
        string path;
        string json = JsonConvert.SerializeObject(prefabJson, Formatting.Indented);

        path = Path.Combine(destPrefabs, prefab.name + ".json");
        StreamWriter writer = new StreamWriter(path);
        writer.Write(json);
        writer.Close();

        Debug.Log(Path.GetFileName(path) + " saved.");
      }

      // save scene
      {
        var sceneJson = DumpScene(nodes);
        string path;
        string sceneName = string.IsNullOrEmpty(scene.name) ? "scene" : scene.name;
        string json = JsonConvert.SerializeObject(sceneJson, Formatting.Indented);

        path = Path.Combine(dest, sceneName + ".json");
        StreamWriter writer = new StreamWriter(path);
        writer.Write(json);
        writer.Close();

        Debug.Log(Path.GetFileName(path) + " saved.");
      }
    }

    void Walk (List<GameObject> _roots, WalkCallback _fn) {
      for (int i = 0; i < _roots.Count; ++i) {
        GameObject gameObject = _roots[i];
        RecurseNode(gameObject, _fn);
      }
    }

    void RecurseNode (GameObject _go, WalkCallback _fn, bool excludeSelf = false) {
      if ( !excludeSelf ) {
        bool walkChildren = _fn(_go);
        if ( !walkChildren ) {
          return;
        }
      }

      foreach (Transform child in _go.transform) {
        RecurseNode(child.gameObject, _fn);
      }
    }

    void Save (string _dest, string _name, GLTF _gltf, List<BufferInfo> _bufferInfos) {
      // create dest directory
      if ( !Directory.Exists(_dest) ) {
        Directory.CreateDirectory(_dest);
      }

      string path;

      // =========================
      // gltf
      // =========================

      string json = JsonConvert.SerializeObject(_gltf, Formatting.Indented);
      // Debug.Log(json);

      path = Path.Combine(_dest, _name + ".gltf");
      StreamWriter writer = new StreamWriter(path);
      writer.Write(json);
      writer.Close();

      Debug.Log(Path.GetFileName(path) + " saved.");

      // =========================
      // buffers (.bin)
      // =========================

      foreach ( BufferInfo buf in _bufferInfos ) {
        path = Path.Combine(_dest, buf.id + ".bin");
        BinaryWriter bwriter = new BinaryWriter(new FileStream(path, System.IO.FileMode.Create));
        bwriter.Write(buf.data);
        bwriter.Close();

        Debug.Log(Path.GetFileName(path) + " saved.");
      }

      // =========================
      // finalize
      // =========================

      // make sure our saved file will show up in Project panel
      AssetDatabase.Refresh();
    }

    void WalkScene (
      Scene _scene,
      List<GameObject> _nodes,
      List<GameObject> _prefabs,
      List<Mesh> _meshes,
      List<GameObject> _animPrefabs
    ) {
      List<GameObject> rootObjects = new List<GameObject>();
      _scene.GetRootGameObjects( rootObjects );

      // collect meshes, skins and animation-clips
      Walk(rootObjects, _go => {

        // =========================
        // get animation prefab
        // =========================

        GameObject prefab = PrefabUtility.GetPrefabParent(_go) as GameObject;
        if ( prefab ) {
          prefab = prefab.transform.root.gameObject;

          // process prefabInfos
          var founded = _animPrefabs.Find(item => {
            return item == prefab;
          });
          if ( founded == null ) {
            bool isAnimPrefab = Utils.IsAnimPrefab(prefab);

            if (isAnimPrefab) {
              _animPrefabs.Add(prefab);
            }
          }
        }

        // =========================
        // get mesh
        // =========================

        Mesh mesh = null;
        MeshFilter meshFilter = _go.GetComponent<MeshFilter>();
        if ( meshFilter ) {
          mesh = meshFilter.sharedMesh;
        }

        if ( mesh != null ) {
          // process meshAssets
          Mesh founded = _meshes.Find(item => {
            return item == mesh;
          });
          if ( founded == null ) {
            _meshes.Add(mesh);
          }
        }

        // continue children or not
        return true;
      });

      // collect prefabs & nodes
      Walk(rootObjects, _go => {

        // =========================
        // get prefabs
        // =========================

        GameObject prefab = PrefabUtility.GetPrefabParent(_go) as GameObject;
        if ( prefab ) {
          prefab = prefab.transform.root.gameObject;

          // check if this is a animPrefab
          var founded = _animPrefabs.Find(item => {
            return item == prefab;
          });
          if ( founded != null ) {
            _nodes.Add(_go);
            return false;
          }

          // process prefabInfos
          founded = _prefabs.Find(item => {
            return item == prefab;
          });
          if ( founded == null ) {
            _prefabs.Add(prefab);
          }

          // add nodes & skip prefab children
          _nodes.Add(_go);
          return false;
        }

        // =========================
        // add nodes
        // =========================

        _nodes.Add(_go);

        return true;
      });
    }
  }
}
