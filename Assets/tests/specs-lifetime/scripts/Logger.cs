using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger : MonoBehaviour {
  bool updated = false;
  bool lateUpdated = false;

  void OnEnable() {
    Debug.Log(gameObject.name + " OnEnable");
  }

  void OnDisable() {
    Debug.Log(gameObject.name + " OnDisable");
  }

  void OnDestroy () {
    Debug.Log(gameObject.name + " OnDestroy");
  }

  void Awake () {
    Debug.Log(gameObject.name + " Awake");
  }

  void Start () {
    Debug.Log(gameObject.name + " Start");
  }

  void Update () {
    if (updated == false) {
      Debug.Log(gameObject.name + " Update");
      updated = true;
    }
  }

  void LateUpdate () {
    if (lateUpdated == false) {
      Debug.Log(gameObject.name + " LateUpdate");
      lateUpdated = true;
    }
  }
}
