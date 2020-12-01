using UnityEngine;
using UnityEngine.UI;

namespace UI {

  public class UIGfxWorldObject : UIGfx {

    private Transform trackedObject;
    private Camera cam;

    public override void Start() {
      base.Start();
    }

    public void SetTracking(Transform trackedObject, Camera cam) {
      this.trackedObject = trackedObject;
      this.cam = cam;
    }

    void LateUpdate() {
      if (trackedObject) {
        var screenPoint = cam.WorldToViewportPoint(trackedObject.position);
        var canvasSize = (Vector3)parentCanvas.pixelRect.size;
        rectTransform.anchoredPosition3D = (Vector3.Scale(screenPoint, canvasSize) - canvasSize / 2) / parentCanvas.scaleFactor;
      }
    }
  }
}