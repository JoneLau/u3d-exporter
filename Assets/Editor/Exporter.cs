﻿using UnityEngine;
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

  public delegate bool WalkCallback(GameObject _go);

  public partial class Exporter {
    public string outputPath;
    public string name;
    public FileMode mode;

    public void Exec() {
      if (!Directory.Exists(this.outputPath)) {
        Debug.LogError("u3d-exporter Failed: Can not find the path \"" + this.outputPath + "\"");
        return;
      }

      // create dest folder
      string name = string.IsNullOrEmpty(this.name) ? "exports" : this.name;
      string dest = Path.Combine(this.outputPath, name);

      if (Directory.Exists(dest)) {
        System.IO.DirectoryInfo di = new DirectoryInfo(dest);
        di.Delete(true);
      }

      // get root objects in scene
      Scene scene = EditorSceneManager.GetActiveScene();
      Dictionary<string, JSON_Asset> assetsJson = new Dictionary<string, JSON_Asset>();

      // get data from scene
      var nodes = new List<GameObject>();
      var prefabs = new List<Object>();
      var modelPrefabs = new List<Object>();
      var materials = new List<Material>();
      var textures = new List<Texture>();

      WalkScene(
        scene,
        nodes,
        prefabs,
        modelPrefabs,
        materials,
        textures
      );

      // DELME {
      // // save meshes
      // var destMeshes = Path.Combine(dest, "meshes");
      // foreach (Mesh mesh in meshes) {
      //   string id = Utils.AssetID(mesh);
      //   GLTF gltf = new GLTF();
      //   gltf.asset = new GLTF_Asset
      //   {
      //     version = "1.0.0",
      //     generator = "u3d-exporter"
      //   };
      //   BufferInfo bufInfo = new BufferInfo
      //   {
      //     id = id,
      //     name = mesh.name
      //   };

      //   DumpMesh(mesh, gltf, bufInfo, 0);
      //   DumpBuffer(bufInfo, gltf);

      //   Save(
      //     destMeshes,
      //     id,
      //     gltf,
      //     new List<BufferInfo> { bufInfo }
      //   );

      //   // add asset to table
      //   assetsJson.Add(id, new JSON_Asset {
      //     type = "mesh",
      //     urls = new Dictionary<string, string> {
      //       { "gltf", "meshes/" + id + ".gltf" },
      //       { "bin", "meshes/" + id + ".bin" }
      //     }
      //   });
      // }
      // } DELME

      // ========================================
      // save animations
      // ========================================

      var destAnims = Path.Combine(dest, "anims");
      foreach (GameObject prefab in prefabs) {
        // skip ModelPrefab
        if (PrefabUtility.GetPrefabType(prefab) == PrefabType.ModelPrefab) {
            Debug.LogWarning("Can not export model prefab " + prefab.name + " in the scene");
            continue;
        }

        // skip non-animation prefab
        bool isAnimPrefab = Utils.IsAnimPrefab(prefab);
        if (isAnimPrefab == false) {
          continue;
        }

        // get animations
        GameObject prefabInst = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        List<AnimationClip> clips = Utils.GetAnimationClips(prefabInst);

        // get joints
        List<GameObject> joints = new List<GameObject>();
        RecurseNode(prefabInst, _go => {
          // this is not a joint
          if (_go.GetComponent<SkinnedMeshRenderer>() != null) {
            return false;
          }

          joints.Add(_go);
          return true;
        });

        // dump animation clips
        if (clips != null) {
          // process AnimationClip(s)
          foreach (AnimationClip clip in clips) {
            string id = Utils.AssetID(clip);
            GLTF gltf = new GLTF();
            gltf.asset = new GLTF_Asset {
              version = "1.0.0",
              generator = "u3d-exporter"
            };
            BufferInfo bufInfo = new BufferInfo {
              id = id,
              name = prefab.name
            };

            AnimData animData = DumpAnimData(prefabInst, clip);
            DumpBufferInfoFromAnimData(animData, bufInfo);

            GLTF_AnimationEx gltfAnim = DumpGltfAnimationEx(animData, joints, 0);
            gltf.animations.Add(gltfAnim);

            DumpBuffer(bufInfo, gltf);

            Save(
              destAnims,
              id,
              gltf,
              new List<BufferInfo> { bufInfo }
            );

            // add asset to table
            try {
              assetsJson.Add(id, new JSON_Asset {
                type = "animation",
                urls = new Dictionary<string, string> {
                  { "gltf", "anims/" + id + ".gltf" },
                  { "bin", "anims/" + id + ".bin" }
                }
              });
            } catch (System.SystemException e) {
              Debug.LogError("Failed to add " + id + " to assets: " + e);
            }
          }
        }

        Object.DestroyImmediate(prefabInst);
      }

      // ========================================
      // save prefabs
      // ========================================

      var destGLTFs = Path.Combine(dest, "gltfs");
      var destPrefabs = Path.Combine(dest, "prefabs");

      // create dest directory
      if (!Directory.Exists(destGLTFs)) {
        Directory.CreateDirectory(destGLTFs);
      }
      if (!Directory.Exists(destPrefabs)) {
        Directory.CreateDirectory(destPrefabs);
      }

      foreach (GameObject prefab in prefabs) {
        string id = Utils.AssetID(prefab);

        // save prefabs
        if (PrefabUtility.GetPrefabType(prefab) == PrefabType.ModelPrefab) {
          Debug.LogWarning("Can not export model prefab " + prefab.name + " in the scene");
          continue;
        }
        var prefabJson = DumpPrefab(prefab);
        string path;
        string json = JsonConvert.SerializeObject(prefabJson, Formatting.Indented);

        path = Path.Combine(destPrefabs, id + ".json");
        StreamWriter writer = new StreamWriter(path);
        writer.Write(json);
        writer.Close();

        // Debug.Log(Path.GetFileName(path) + " saved.");

        // add asset to table
        assetsJson.Add(id, new JSON_Asset {
          type = "prefab",
          urls = new Dictionary<string, string> {
            { "json", "prefabs/" + id + ".json" },
          }
        });

        // save model prefab (as gltf)

      }
      foreach (GameObject modelPrefab in modelPrefabs) {
        string id = Utils.AssetID(modelPrefab);
        // save model prefabs
        GLTF gltf = new GLTF();
        gltf.asset = new GLTF_Asset {
          version = "1.0.0",
          generator = "u3d-exporter"
        };
        BufferInfo bufInfo = new BufferInfo {
          id = id,
          name = modelPrefab.name
        };

        bool isAnimPrefab = Utils.IsAnimPrefab(modelPrefab);
        if (isAnimPrefab) {
          DumpSkinningModel(modelPrefab, gltf, bufInfo);
          DumpBuffer(bufInfo, gltf);
        } else {
          DumpModel(modelPrefab, gltf, bufInfo);
          DumpBuffer(bufInfo, gltf);
        }

        Save(
          destGLTFs,
          id,
          gltf,
          new List<BufferInfo> { bufInfo }
        );

        // add asset to table
        assetsJson.Add(id, new JSON_Asset {
          type = "gltf",
          urls = new Dictionary<string, string> {
            { "gltf", "gltfs/" + id + ".gltf" },
            { "bin", "gltfs/" + id + ".bin" }
          }
        });
      }

      // ========================================
      // save textures
      // ========================================

      var destTextures = Path.Combine(dest, "textures");
      // create dest directory
      if (!Directory.Exists(destTextures)) {
        Directory.CreateDirectory(destTextures);
      }
      foreach (Texture tex in textures) {
        var textureJson = DumpTexture(tex);
        string path;
        string json = JsonConvert.SerializeObject(textureJson, Formatting.Indented);
        string id = Utils.AssetID(tex);

        // json
        path = Path.Combine(destTextures,  id + ".json");
        StreamWriter writer = new StreamWriter(path);
        writer.Write(json);
        writer.Close();

        // image
        string assetPath = AssetDatabase.GetAssetPath(tex);
        path = Path.Combine(destTextures,  id + Utils.AssetExt(tex));
        File.Copy(assetPath, path);

        // Debug.Log(Path.GetFileName(path) + " saved.");

        // add asset to table
        assetsJson.Add(id, new JSON_Asset {
          type = "texture-2d",
          urls = new Dictionary<string, string> {
            { "json", "textures/" + id + ".json" },
            { "image", "textures/" + id + Utils.AssetExt(tex) },
          }
        });
      }

      // ========================================
      // save materials
      // ========================================

      var destMaterials = Path.Combine(dest, "materials");
      // create dest directory
      if (!Directory.Exists(destMaterials)) {
        Directory.CreateDirectory(destMaterials);
      }
      foreach (Material mat in materials) {
        var materialJson = DumpMaterial(mat);
        if ( materialJson == null) {
          continue;
        }

        string path;
        string json = JsonConvert.SerializeObject(materialJson, Formatting.Indented);
        string id = Utils.AssetID(mat);

        // json
        path = Path.Combine(destMaterials,  id + ".json");
        StreamWriter writer = new StreamWriter(path);
        writer.Write(json);
        writer.Close();

        // Debug.Log(Path.GetFileName(path) + " saved.");

        // add asset to table
        assetsJson.Add(id, new JSON_Asset {
          type = "material",
          urls = new Dictionary<string, string> {
            { "json", "materials/" + id + ".json" },
          }
        });
      }

      // ========================================
      // save assets
      // ========================================

      {
        string path = Path.Combine(dest, "assets.json");
        string json = JsonConvert.SerializeObject(assetsJson, Formatting.Indented);

        StreamWriter writer = new StreamWriter(path);
        writer.Write(json);
        writer.Close();
      }

      // ========================================
      // save scene
      // ========================================

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

    void Walk(List<GameObject> _roots, WalkCallback _fn) {
      for (int i = 0; i < _roots.Count; ++i) {
        GameObject gameObject = _roots[i];
        RecurseNode(gameObject, _fn);
      }
    }

    void RecurseNode(GameObject _go, WalkCallback _fn, bool excludeSelf = false) {
      if (!excludeSelf) {
        bool walkChildren = _fn(_go);
        if (!walkChildren) {
          return;
        }
      }

      foreach (Transform child in _go.transform) {
        RecurseNode(child.gameObject, _fn);
      }
    }

    void Save(string _dest, string _name, GLTF _gltf, List<BufferInfo> _bufferInfos) {
      // create dest directory
      if (!Directory.Exists(_dest)) {
        Directory.CreateDirectory(_dest);
      }

      string path;

      // =========================
      // gltf
      // =========================

      string json = JsonConvert.SerializeObject(_gltf, Formatting.Indented);
      path = Path.Combine(_dest, _name + ".gltf");
      StreamWriter writer = new StreamWriter(path);
      writer.Write(json);
      writer.Close();

      // Debug.Log(Path.GetFileName(path) + " saved.");

      // =========================
      // buffers (.bin)
      // =========================

      foreach (BufferInfo buf in _bufferInfos) {
        path = Path.Combine(_dest, buf.id + ".bin");
        BinaryWriter bwriter = new BinaryWriter(new FileStream(path, System.IO.FileMode.Create));
        bwriter.Write(buf.data);
        bwriter.Close();

        // Debug.Log(Path.GetFileName(path) + " saved.");
      }

      // =========================
      // finalize
      // =========================

      // make sure our saved file will show up in Project panel
      AssetDatabase.Refresh();
    }

    void WalkScene(
      Scene _scene,
      List<GameObject> _nodes,
      List<Object> _prefabs,
      List<Object> _modelPrefabs,
      List<Material> _materials,
      List<Texture> _textures
    ) {
      List<GameObject> rootObjects = new List<GameObject>();
      _scene.GetRootGameObjects(rootObjects);

      // collect meshes, skins, materials, textures and animation-clips
      Walk(rootObjects, _go => {
        // =========================
        // get material & textures
        // =========================

        Renderer renderer = _go.GetComponent<Renderer>();
        if (renderer) {
          foreach (Material mat in renderer.sharedMaterials) {
            if (mat == null) {
              Debug.LogWarning("Null material in " + _go.name);
              continue;
            }
            Material foundedMaterial = _materials.Find(m => {
              return m == mat;
            });
            if (foundedMaterial == null) {
              _materials.Add(mat);

              // handle textures
              List<Texture> textures = Utils.GetTextures(mat);
              foreach (Texture tex in textures) {
                Texture foundedTexture = _textures.Find(t => {
                  return t == tex;
                });
                if (foundedTexture == null) {
                  _textures.Add(tex);
                }
              }
            }
          }
        }

        // =========================
        // get model prefab from mesh
        // =========================

        Mesh mesh = null;
        MeshFilter meshFilter = _go.GetComponent<MeshFilter>();
        if (meshFilter) {
          mesh = meshFilter.sharedMesh;
        }
        SkinnedMeshRenderer skinnedMeshRenderer = _go.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer) {
          mesh = skinnedMeshRenderer.sharedMesh;
        }

        if (mesh != null) {
          var path = AssetDatabase.GetAssetPath(mesh);
          var prefab = AssetDatabase.LoadMainAssetAtPath(path);
          if (prefab) {
            var type = PrefabUtility.GetPrefabType(prefab);
            if (type != PrefabType.ModelPrefab) {
              Debug.LogWarning("Can not export mesh " + mesh.name + ": it is not a model prefab.");
            }

            // check if prefab already exists
            var founded = _modelPrefabs.Find(item => {
              return item == prefab;
            });
            if (founded == null) {
              _modelPrefabs.Add(prefab);
            }
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

        var type = PrefabUtility.GetPrefabType(_go);
        if (type != PrefabType.None) {
          var prefab = Utils.GetPrefabAsset(_go);

          // check if prefab already exists
          var founded = _prefabs.Find(item => {
            return item == prefab;
          });
          if (founded == null) {
            _prefabs.Add(prefab);
          }

          // add nodes & skip prefab children
          _nodes.Add(_go);

          // recurse prefab child see if any nested prefab in it.
          RecurseNode(_go, _childGO => {
            var childRoot = PrefabUtility.FindPrefabRoot(_childGO);
            if (childRoot == _go) {
              return false;
            }

            _nodes.Add(_childGO);

            var childType = PrefabUtility.GetPrefabType(_childGO);
            if (childType != PrefabType.None) {
              var childPrefab = Utils.GetPrefabAsset(_childGO);

              // check if prefab already exists
              founded = _prefabs.Find(item => {
                return item == childPrefab;
              });
              if (founded == null) {
                _prefabs.Add(childPrefab);
              }
            }

            return true;
          }, true);

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