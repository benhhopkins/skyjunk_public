using System.Collections.Generic;
using UnityEngine;
using Utility;
using DG.Tweening;

public class GraphicalSpinner : MonoBehaviour {
  public Transform spinObject;
  public IntVariable intVariable = null;
  public float rotationSpeed = 1;

  private int lastValue;

  void LateUpdate() {
    if (intVariable.Value != lastValue) {
      lastValue = intVariable.Value;
      spinObject.DORotate(spinObject.rotation.eulerAngles +
            Vector3.up * -800, .5f).SetEase(Ease.OutQuad);
    }

    spinObject.Rotate(Vector3.up * rotationSpeed, Space.World);
  }

}