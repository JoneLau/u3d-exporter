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
    public List<int> children = new List<int>();
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
    public JSON_Asset prefab;
    public float[] translation = new float[3] {0,0,0};
    public float[] rotation = new float[4] {0,0,0,1};
    public float[] scale = new float[3] {1,1,1};
    public List<JSON_Component> components;
    public List<int> children;

    public bool ShouldSerializename () {
      return string.IsNullOrEmpty(name) == false;
    }

    public bool ShouldSerializeprefab () {
      return prefab != null;
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

  // =========================
  // JSON_Asset
  // =========================

  [System.Serializable]
  public class JSON_Asset {
    public string type;
    public Dictionary<string,string> urls = new Dictionary<string,string>();
  }

  // =========================
  // JSON_Texture
  // =========================

  [System.Serializable]
  public class JSON_Texture {
    public string type = "2d";
    public int anisotropy = -1;
    public string minFilter;
    public string magFilter;
    public string mipFilter;
    public string wrapS;
    public string wrapT;
    public string wrapR;
    public bool mipmap = true;

    public bool ShouldSerializeanisotropy () {
      return anisotropy != -1;
    }

    public bool ShouldSerializeminFilter () {
      return string.IsNullOrEmpty(minFilter) == false;
    }

    public bool ShouldSerializemagFilter () {
      return string.IsNullOrEmpty(magFilter) == false;
    }

    public bool ShouldSerializemipFilter () {
      return string.IsNullOrEmpty(mipFilter) == false;
    }

    public bool ShouldSerializewrapS () {
      return string.IsNullOrEmpty(wrapS) == false;
    }

    public bool ShouldSerializewrapT () {
      return string.IsNullOrEmpty(wrapT) == false;
    }

    public bool ShouldSerializewrapR () {
      return string.IsNullOrEmpty(wrapR) == false;
    }

    public bool ShouldSerializemipmap () {
      return mipmap == false;
    }
  }
}