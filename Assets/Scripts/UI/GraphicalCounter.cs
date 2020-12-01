using System.Collections.Generic;
using UnityEngine;
using Utility;
using DG.Tweening;

public class GraphicalCounter : MonoBehaviour {
  public List<Transform> counterTransforms = new List<Transform>();
  public IntVariable intVariable = null;
  public float rotationSpeed = 1;

  private int lastValue;

  void LateUpdate() {
    if (intVariable.Value != lastValue) {
      lastValue = intVariable.Value;
      SetValue(lastValue);
    }

    for (int i = 0; i < counterTransforms.Count; i++) {
      if (counterTransforms[i].gameObject.activeSelf) {
        counterTransforms[i].Rotate(Vector3.up * rotationSpeed, Space.World);
      }
    }
  }

  private void SetValue(int value) {
    for (int i = 0; i < counterTransforms.Count; i++) {
      if (i < value) {
        if (!counterTransforms[i].gameObject.activeSelf) {
          counterTransforms[i].gameObject.SetActive(true);
          counterTransforms[i].DORotate(counterTransforms[i].rotation.eulerAngles +
            Vector3.up * -800, .5f).SetEase(Ease.OutQuad);
        }
      } else
        counterTransforms[i].gameObject.SetActive(false);
    }
  }
}