using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple : MonoBehaviour {
  public Material mat;
  private Vector3 startVertex;
  private Vector3 mousePos;

  void Update() {
    mousePos = Input.mousePosition;
    if (Input.GetKeyDown(KeyCode.Space)) {
      startVertex = new Vector3(mousePos.x / Screen.width, mousePos.y / Screen.height, 0);
      Debug.Log(mousePos.x + ", " + mousePos.y);
    }

  }

  void OnRenderObject() {
    if (!mat) {
      Debug.LogError("Please Assign a material on the inspector");
      return;
    }
    GL.PushMatrix();
    mat.SetPass(0);
    GL.LoadOrtho();
    GL.Begin(GL.LINES);
    GL.Color(Color.red);
    GL.Vertex(new Vector3(10.0f / Screen.width, 10.0f / Screen.height, 0));
    GL.Vertex(new Vector3(10.0f / Screen.width, 100.0f / Screen.height, 0));

    GL.Vertex(new Vector3(10.0f / Screen.width, 100.0f / Screen.height, 0));
    GL.Vertex(new Vector3(100.0f / Screen.width, 100.0f / Screen.height, 0));

    GL.Vertex(new Vector3(100.0f / Screen.width, 100.0f / Screen.height, 0));
    GL.Vertex(new Vector3(100.0f / Screen.width, 10.0f / Screen.height, 0));

    GL.Vertex(new Vector3(100.0f / Screen.width, 10.0f / Screen.height, 0));
    GL.Vertex(new Vector3(10.0f / Screen.width, 10.0f / Screen.height, 0));

    GL.End();
    GL.PopMatrix();
  }
}
