using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;
using UnityEditor.SceneManagement;

using System.Collections;
using System.Collections.Generic;

namespace exsdk {
  public class GLTFExportWindow : EditorWindow {
    public string outputPath = "";
    public string projectName = "";
    public FileMode mode = FileMode.Mixed;

    [MenuItem("Window/u3d-exporter")]
    static void Open() {
      GLTFExportWindow window = (GLTFExportWindow)EditorWindow.GetWindow(typeof(GLTFExportWindow));
      window.titleContent = new GUIContent("u3d-exporter");
      window.minSize = new Vector2(200, 200);
      window.Show();
    }

    void OnEnable() {
      string defaultPath = System.IO.Path.GetDirectoryName(Application.dataPath);
      this.outputPath = EditorPrefs.GetString("outputPath", defaultPath);
      this.projectName = EditorPrefs.GetString("projectName", Application.productName);

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

      this.outputPath = EditorGUILayout.TextField("Output Path", this.outputPath);

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
        EditorPrefs.SetString("projectName", projName);
      }

      // =========================
      // Mode
      // =========================

      this.mode = (FileMode)EditorGUILayout.EnumPopup("Mode", this.mode);

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
    }
  }
}
