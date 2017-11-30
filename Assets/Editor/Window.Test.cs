using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;
using UnityEditor.SceneManagement;

using System.Collections;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace exsdk {
  public class TestWindow : EditorWindow {
    public Object target;

    [MenuItem("Window/test")]
    static void Open() {
      TestWindow window = (TestWindow)EditorWindow.GetWindow(typeof(TestWindow));
      window.titleContent = new GUIContent("test");
      window.minSize = new Vector2(200, 200);
      window.Show();
    }

    void OnEnable() {
      this.Repaint();
    }

    void OnGUI() {
      EditorGUIUtility.labelWidth = 100.0f;

      // =========================
      // Options
      // =========================

      GUILayout.Label("Options", EditorStyles.boldLabel);

      // #########################
      // Start
      // #########################

      GUIStyle style = EditorStyles.inspectorDefaultMargins;
      EditorGUILayout.BeginVertical(style, new GUILayoutOption[0]);

      // =========================
      // Test Button
      // =========================

      this.target = EditorGUILayout.ObjectField("Target", this.target, typeof(Object), true);

      EditorGUILayout.Space();
      EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
      EditorGUILayout.Space();
      GUILayout.FlexibleSpace();
      if (GUILayout.Button("Test", "LargeButton", GUILayout.MaxWidth(200))) {
        if (this.target) {
          var rectTrans = (this.target as GameObject).GetComponent<RectTransform>();
          Debug.Log("anchoredPosition: " + rectTrans.anchoredPosition);
          Debug.Log("anchorMax: " + rectTrans.anchorMax);
          Debug.Log("anchorMin: " + rectTrans.anchorMin);
          Debug.Log("offsetMax: " + rectTrans.offsetMax);
          Debug.Log("offsetMin: " + rectTrans.offsetMin);
          Debug.Log("pivot: " + rectTrans.pivot);
          Debug.Log("rect: " + rectTrans.rect);
          Debug.Log("sizeDelta: " + rectTrans.sizeDelta);

          // var path = AssetDatabase.GetAssetPath(this.target);
          // var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
          // var prefabType = PrefabUtility.GetPrefabType(mainAsset);

          // // var obj = PrefabUtility.GetPrefabObject(this.target);
          // var obj = PrefabUtility.FindPrefabRoot(this.target as GameObject);
          // var asset = PrefabUtility.GetPrefabParent(obj);
          // var path = AssetDatabase.GetAssetPath(asset);
          // Debug.Log(path);

          // Debug.Log("prefab type: " + prefabType);
          // var go = PrefabUtility.InstantiatePrefab(mainAsset);
        }
      }
      GUILayout.FlexibleSpace();
      EditorGUILayout.Space();
      EditorGUILayout.EndHorizontal();

      // #########################
      // End
      // #########################

      EditorGUILayout.EndVertical();
    }
  }
}