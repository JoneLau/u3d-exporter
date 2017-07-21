using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using System.IO;

namespace exsdk {
  public partial class Exporter {
    // -----------------------------------------
    // DumpAnim
    // -----------------------------------------

    void DumpAnim (GameObject _animPrefab, GLTF _gltf, BufferInfo _bufInfo) {
      // get joints
      List<GameObject> joints = new List<GameObject>();
      RecurseNode(_animPrefab, _go => {
        // this is not a joint
        if ( _go.GetComponent<SkinnedMeshRenderer>() != null ) {
          return false;
        }

        joints.Add(_go);
        return true;
      });

      // dump meshes
      int accOffset = 0;
      foreach (Transform child in _animPrefab.transform) {
        SkinnedMeshRenderer smr = child.GetComponent<SkinnedMeshRenderer>();
        if ( smr == null ) {
          continue;
        }

        // dump mesh
        accOffset += _bufInfo.GetAccessorCount();
        DumpMesh(smr.sharedMesh, _gltf, _bufInfo, accOffset);

        // dump skin
        int accBindposesIdx = _bufInfo.GetAccessorCount() - 1;
        GameObject rootBone = Utils.GetRootBone(smr, _animPrefab).gameObject;

        GLTF_Skin gltfSkin = DumpGtlfSkin(smr, joints, rootBone, accBindposesIdx);
        if ( gltfSkin != null ) {
          _gltf.skins.Add(gltfSkin);
        }
      }
    }

    // -----------------------------------------
    // DumpGltfSkin
    // -----------------------------------------

    GLTF_Skin DumpGtlfSkin (SkinnedMeshRenderer _smr, List<GameObject> _joints, GameObject _rootBone, int _accBindposes) {
      Mesh mesh = _smr.sharedMesh;
      if ( mesh.bindposes.Length != _smr.bones.Length ) {
        Debug.LogWarning("Failed to dump gltf-skin from " + _smr.name + ", please turn off \"Optimize Game Objects\" in the \"Rig\".");
        return null;
      }

      GLTF_Skin gltfSkin = new GLTF_Skin();

      gltfSkin.name = _smr.name;
      gltfSkin.inverseBindMatrices = _accBindposes;
      gltfSkin.skeleton = _joints.IndexOf(_rootBone);
      gltfSkin.joints = new int[_smr.bones.Length];

      for ( int i = 0; i < _smr.bones.Length; ++i ) {
        gltfSkin.joints[i] = _joints.IndexOf(_smr.bones[i].gameObject);
      }

      return gltfSkin;
    }
  }
}