using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace exsdk {
  [CustomEditor(typeof(ScriptComponent))]
  public class ScriptComponentEditor : Editor {
    ScriptComponent show;
    ScriptCompDesc desc;
    private List<string> properties = new List<string>();

    private void OnEnable() {
      show = (ScriptComponent)target;
      show.resetProperties();
      desc = show.desc;
    }

    public override void OnInspectorGUI() {   
      EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
      show.desc = (ScriptCompDesc)EditorGUILayout.ObjectField("Script",show.desc, typeof(ScriptCompDesc));
      EditorGUILayout.EndHorizontal();

      if (GUI.changed) {
        if (show.desc !=desc) {
          desc = show.desc;
          show.properties = null;
          show.resetProperties();
        }
      }

      if (desc != null) {
        properties.Clear();
        if (show.properties != null) {
          foreach (KeyValuePair<string, object> item in show.properties) {
            properties.Add(item.Key);
          }
        }

        foreach (string name in properties) {
          EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
          EditorGUILayout.LabelField(name, GUILayout.MaxWidth(50));

          if (show.properties[name].GetType() == typeof(string)) {
            show.properties[name] = EditorGUILayout.TextField(show.properties[name].ToString());
          } else if (show.properties[name].GetType() == typeof(int)) {
            show.properties[name] = EditorGUILayout.IntField(int.Parse(show.properties[name].ToString()));
          } else if (show.properties[name].GetType() == typeof(float)) {
            show.properties[name] = EditorGUILayout.FloatField(float.Parse(show.properties[name].ToString()));
          } else if (show.properties[name].GetType() == typeof(bool)) {
            show.properties[name] = EditorGUILayout.Toggle(bool.Parse(show.properties[name].ToString()));
          }
          EditorGUILayout.EndHorizontal();
        }
      }
    }
  }
}
