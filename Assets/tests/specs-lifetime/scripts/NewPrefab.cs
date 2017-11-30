using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPrefab : MonoBehaviour {
  public GameObject prefab;

  // Use this for initialization
  void Start () {
    var pobj = Object.Instantiate(this.prefab);
    Debug.Log("prefab created");
  }

  // Update is called once per frame
  void Update () {
  }
}
