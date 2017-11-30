using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewComp : MonoBehaviour {
  // Use this for initialization
  void Start () {
    this.gameObject.AddComponent<Simple>();
    var simple = this.gameObject.GetComponent<Simple>();
    Debug.Log(simple + " added");

    Object.Destroy(simple);
    simple = this.gameObject.GetComponent<Simple>();
    Debug.Log(simple + " destroyed");
  }

  // Update is called once per frame
  void Update () {
  }
}
