using UnityEngine;

public class LookTarget : MonoBehaviour {
  public Transform trackObject;
  public Vector3 offset;

  void LateUpdate() {
    if (trackObject != null)
      transform.position = trackObject.position + offset;
  }
}