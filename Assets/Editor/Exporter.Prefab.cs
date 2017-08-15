using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using System.IO;

namespace exsdk {
  public partial class Exporter {

    // -----------------------------------------
    // DumpPrefab
    // -----------------------------------------

    JSON_Prefab DumpPrefab (GameObject _prefab) {
      JSON_Prefab result = new JSON_Prefab();
      List<GameObject> nodes = new List<GameObject>();

      // collect meshes, skins and animation-clips
      Walk(new List<GameObject>{_prefab}, _go => {
        nodes.Add(_go);
        return true;
      });

      // dump entities
      foreach ( GameObject go in nodes ) {
        JSON_Entity ent = DumpEntity(go, nodes);
        result.entities.Add(ent);
      }

      return result;
    }
  }
}