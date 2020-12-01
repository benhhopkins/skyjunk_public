using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility {
  public class PreviewRenderer : MonoBehaviour {

    [SerializeField] private Camera previewCamera = null;
    public Camera PreviewCamera { get => previewCamera; }
    [SerializeField] private RenderTexture previewTexture = null;
    public RenderTexture PreviewTexture { get => previewTexture; }

    public float margin = 0.1f;
    public float distance = 10;
    public float height = 1;
    public float speed = 1;

    private CapsuleCollider lookObject = null;
    private float timer = 0;

    private static PreviewRenderer _instance;
    public static PreviewRenderer I {
      get {
        if (_instance == null)
          _instance = GameObject.FindObjectOfType<PreviewRenderer>();
        return _instance;
      }
    }

    void Start() {
      if (_instance != null && _instance != this) {
        GameObject.Destroy(gameObject);
        return;
      }
      _instance = this;
    }

    public void SetSizeByCollider(CapsuleCollider collider) {
      lookObject = collider;
      var size = Mathf.Max(collider.height / 2, collider.radius) + margin;
      previewCamera.orthographicSize = size;
    }

    void Update() {
      if (lookObject) {
        timer += Time.unscaledDeltaTime * speed;
        float x = -Mathf.Cos(timer) * distance;
        float z = Mathf.Sin(timer) * distance;
        Vector3 pos = new Vector3(x, height, z);
        previewCamera.transform.position = pos + lookObject.transform.position;
        previewCamera.transform.LookAt(lookObject.center);
      }
    }
  }
}