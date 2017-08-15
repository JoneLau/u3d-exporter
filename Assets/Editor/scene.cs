using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace exsdk {
  // =========================
  // JSON_Scene
  // =========================

  [System.Serializable]
  public class JSON_Scene {
    public List<JSON_Entity> entities = new List<JSON_Entity>();
  }

  // =========================
  // JSON_Prefab
  // =========================

  [System.Serializable]
  public class JSON_Prefab {
    public List<JSON_Entity> entities = new List<JSON_Entity>();
  }

  // =========================
  // JSON_Entity
  // =========================

  [System.Serializable]
  public class JSON_Entity {
    // basic
    public string name;
    public string prefab;
    public float[] translation = new float[3] {0,0,0};
    public float[] rotation = new float[4] {0,0,0,1};
    public float[] scale = new float[3] {1,1,1};
    public List<JSON_Component> components;
    public List<int> children;

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }

    public bool ShouldSerializeprefab () {
      return string.IsNullOrEmpty(prefab) == false;
    }

    public bool ShouldSerializetranslation () {
      return translation[0] != 0 || translation[1] != 0 || translation[2] != 0;
    }

    public bool ShouldSerializerotation () {
      return rotation[0] != 0 || rotation[1] != 0 || rotation[2] != 0 || rotation[3] != 1;
    }

    public bool ShouldSerializescale () {
      return scale[0] != 1 || scale[1] != 1 || scale[2] != 1;
    }

    public bool ShouldSerializecomponents () {
      return components != null && components.Count != 0;
    }

    public bool ShouldSerializechildren () {
      return children != null && children.Count != 0;
    }
  }

  // =========================
  // JSON_Component
  // =========================

  [System.Serializable]
  public class JSON_Component {
    public string type;
    public Dictionary<string,object> properties = new Dictionary<string,object>();

    public bool ShouldSerializeproperties () {
      return properties != null && properties.Count != 0;
    }
  }
}