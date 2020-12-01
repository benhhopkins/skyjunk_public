using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility {
  public class LookOrbit : MonoBehaviour {

    public float distance;
    public float height;
    public Transform lookLocation = null;
    public float speed;

    private float timer = 0;

    void Update() {
      timer += Time.unscaledDeltaTime * speed;
      Rotate();
      transform.LookAt(lookLocation);
    }

    private void Rotate() {
      float x = -Mathf.Cos(timer) * distance;
      float z = Mathf.Sin(timer) * distance;
      Vector3 pos = new Vector3(x, height, z);
      transform.position = pos + lookLocation.position;
    }
  }
}