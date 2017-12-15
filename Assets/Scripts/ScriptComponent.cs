using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScriptComponent : MonoBehaviour {

  public ScriptCompDesc desc;
  public Dictionary<string, object> properties;

  public void resetProperties() {
    if (desc == null) {
      return;
    }

    if (desc.properties.Count <= 0) {
      return;
    }

    if (properties == null) {
      properties = new Dictionary<string, object>();

      foreach (var item in desc.properties) {
        properties[item.name] = newProperty(item.type);
      }
    } else {
      Dictionary<string, object> oldproperties = new Dictionary<string, object>();

      foreach (KeyValuePair<string, object> item in properties) {
        oldproperties[item.Key] = item.Value;
      }

      properties.Clear();

      for (int i = 0; i < desc.properties.Count; i++) {
        string name = desc.properties[i].name;
        if (oldproperties.ContainsKey(name)) {
          properties[name] = oldproperties[name];
        } else {
          properties[name] = newProperty(desc.properties[i].type);
        }
      }
    }
  }

  public static object newProperty(ScriptCompDescProperty.PropMode type) {
    if (type == ScriptCompDescProperty.PropMode.Int) {
      return 0;
    } else if (type == ScriptCompDescProperty.PropMode.Float) {
      return 0f;
    } else if (type == ScriptCompDescProperty.PropMode.Bool) {
      return false;
    } else {
      return "";
    }
  }
}
