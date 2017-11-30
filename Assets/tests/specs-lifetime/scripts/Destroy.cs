using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour {
  public GameObject target;

  void Start () {
  }

  void Update () {
    if (target) {
      Object.Destroy(target);
      Debug.Log("Destroy Updated");
    }
  }

  void LateUpdate () {
    if (target) {
      Debug.Log("Destroy LateUpdated");
    }
  }
}
