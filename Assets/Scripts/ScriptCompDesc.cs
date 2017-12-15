using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "CustomScriptComp/Editor")]
public class ScriptCompDesc : ScriptableObject {
  public List<ScriptCompDescProperty> properties = new List<ScriptCompDescProperty>();
}
[System.Serializable]
public class ScriptCompDescProperty {
  public enum PropMode {
    String,
    Int,
    Float,
    Bool,
  }

  public string name;
  public PropMode type;
}

