﻿using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;

using System.Collections;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

public class ExportSettings {
}

namespace exsdk {
  public class GLTFExportWindow : EditorWindow {
    public string outputPath = "";
    public string projectName = "";
    public FileMode mode = FileMode.Mixed;
    public List<SceneAsset> scenes = new List<SceneAsset>();

    ReorderableList reorderableList = null;
    bool jsonDirty = false;

    [MenuItem("Window/u3d-exporter")]
    static void Open() {
      GLTFExportWindow window = (GLTFExportWindow)EditorWindow.GetWindow(typeof(GLTFExportWindow));
      window.titleContent = new GUIContent("u3d-exporter");
      window.minSize = new Vector2(200, 200);
      window.Show();
    }

    void OnEnable() {
      string projectPath = Path.GetDirectoryName(Application.dataPath);
      string settingsPath = Path.Combine(projectPath, "u3d-exporter.json");

      // read u3d-exporter.json
      JSON_ExportSettings settings = null;
      try {
        using (StreamReader r = new StreamReader(settingsPath)) {
          string json = r.ReadToEnd();
          settings = JsonConvert.DeserializeObject<JSON_ExportSettings>(json);
        }
      } catch (System.Exception) {
        settings = new JSON_ExportSettings();
      }

      // outputPath
      this.outputPath = settings.outputPath;
      if (string.IsNullOrEmpty(this.outputPath)) {
        this.outputPath = projectPath;
      }

      // projectName
      this.projectName = settings.projectName;
      if (string.IsNullOrEmpty(this.projectName)) {
        this.projectName = "out";
      }

      // scenes
      for (int i = 0; i < settings.scenes.Count; ++i) {
        string scenePath = AssetDatabase.GUIDToAssetPath(settings.scenes[i]);
        SceneAsset asset = AssetDatabase.LoadAssetAtPath(scenePath, typeof(SceneAsset)) as SceneAsset;
        if (asset) {
          this.scenes.Add(asset);
        }
      }

      this.reorderableList = new ReorderableList(this.scenes, typeof(SceneAsset), true, true, true, true);
      this.reorderableList.drawHeaderCallback = (Rect rect) => {
        EditorGUI.LabelField(rect, "Scenes:", EditorStyles.boldLabel);
      };
      this.reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
        object asset = this.reorderableList.list[index];
        rect.y += 2;
        rect.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.BeginChangeCheck();
        asset = EditorGUI.ObjectField(rect, asset as Object, typeof(SceneAsset), false);
        if (EditorGUI.EndChangeCheck()) {
          this.reorderableList.list[index] = asset;
          this.jsonDirty = true;
        }
      };

      this.Repaint();
    }

    void Export() {
      if (
        EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneLinux &&
        EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneLinux64 &&
        EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneLinuxUniversal &&
        EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneOSXIntel &&
        EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneOSXIntel64 &&
        EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneOSXUniversal &&
        EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows &&
        EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64
      ) {
        EditorUtility.DisplayDialog("Error", "Your build target must be set to standalone", "Okay");
        return;
      }

      Exporter exporter = new Exporter();
      exporter.outputPath = this.outputPath;
      exporter.name = this.projectName;
      exporter.mode = this.mode;

      exporter.Exec();
    }

    void Browse() {
      string result = EditorUtility.OpenFolderPanel("Choose your export Directory", this.outputPath, "");
      if (string.IsNullOrEmpty(result) == false) {
        this.outputPath = result;
        EditorPrefs.SetString("outputPath", result);

        this.Repaint();
      }
      GUIUtility.ExitGUI();
    }

    void Explore(string path) {
      bool openInsidesOfFolder = false;

      if (SystemInfo.operatingSystem.IndexOf("Windows") != -1) {
        string winPath = path.Replace("/", "\\");

        if (System.IO.Directory.Exists(winPath)) {
          openInsidesOfFolder = true;
        }

        try {
          System.Diagnostics.Process.Start(
            "explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath
          );
        } catch (System.ComponentModel.Win32Exception e) {
          e.HelpLink = "";
        }
      } else {
        if (System.IO.Directory.Exists(path)) {
          openInsidesOfFolder = true;
        }

        string arguments = (openInsidesOfFolder ? "" : "-R ") + path;

        try {
          System.Diagnostics.Process.Start("open", arguments);
        } catch (System.ComponentModel.Win32Exception e) {
          e.HelpLink = "";
        }
      }
    }

    void OnGUI() {
      EditorGUIUtility.labelWidth = 100.0f;
      // EditorGUIUtility.fieldWidth = fieldWidth;

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
      // Output Path
      // =========================

      string outputPath = EditorGUILayout.TextField("Output Path", this.outputPath);
      if (outputPath != this.outputPath) {
        this.outputPath = outputPath;
        this.jsonDirty = true;
      }

      // =========================
      // Browse
      // =========================

      EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
      GUILayout.FlexibleSpace();
      if (GUILayout.Button("Open...", GUILayout.MaxWidth(80))) {
        this.Explore(this.outputPath);
      }
      if (GUILayout.Button("Browse...", GUILayout.MaxWidth(80))) {
        this.Browse();
      }
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.Space();

      // =========================
      // Project Name
      // =========================

      string projName = EditorGUILayout.TextField("Project Name", this.projectName);
      if (projName != this.projectName) {
        this.projectName = projName;
        this.jsonDirty = true;
      }

      // =========================
      // Mode
      // =========================

      this.mode = (FileMode)EditorGUILayout.EnumPopup("Mode", this.mode);

      // =========================
      // Scenes
      // =========================

      if (this.reorderableList != null) {
        this.reorderableList.DoLayoutList();
      }

      // =========================
      // Export Button
      // =========================

      EditorGUILayout.Space();
      EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
      EditorGUILayout.Space();
      GUILayout.FlexibleSpace();
      if (GUILayout.Button("Export", "LargeButton", GUILayout.MaxWidth(200))) {
        this.Export();
      }
      GUILayout.FlexibleSpace();
      EditorGUILayout.Space();
      EditorGUILayout.EndHorizontal();

      // #########################
      // End
      // #########################

      EditorGUILayout.EndVertical();

      if (this.jsonDirty) {
        JSON_ExportSettings settings = new JSON_ExportSettings();
        settings.projectName = this.projectName;
        settings.outputPath = this.outputPath;
        settings.mode = this.mode;

        for (int i = 0; i < this.scenes.Count; ++i) {
          string path = AssetDatabase.GetAssetPath(this.scenes[i]);
          settings.scenes.Add(AssetDatabase.AssetPathToGUID(path));
        }

        // save json
        string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        string projectPath = Path.GetDirectoryName(Application.dataPath);
        string settingsPath = Path.Combine(projectPath, "u3d-exporter.json");
        StreamWriter writer = new StreamWriter(settingsPath);
        writer.Write(json);
        writer.Close();

        this.jsonDirty = false;
      }
    }
  }
}
