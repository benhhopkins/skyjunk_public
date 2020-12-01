using UnityEngine;

public class CameraHelper : MonoBehaviour {

  void OnEnable() {
    GameManager.I.CMFreeLook.ForceCameraPosition(transform.position, transform.rotation);
  }
}