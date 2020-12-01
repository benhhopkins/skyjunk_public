using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(ORDER_EXECUTION)]
public class InterpolationObjectController : MonoBehaviour {
  public const int ORDER_EXECUTION = InterpolationManager.ORDER_EXECUTION - 1;

  private TransformData[] _transforms;
  private int _index;

  private WaitForEndOfFrame waitForEndOfFrame;
  private WaitForFixedUpdate waitForFixedUpdate;

  private void Awake() {
    waitForEndOfFrame = new WaitForEndOfFrame();
    waitForFixedUpdate = new WaitForFixedUpdate();
    StartCoroutine(WaitForEndOfFrame());
    StartCoroutine(WaitForFixedUpdate());
  }

  private void OnEnable() {
    ResetTransforms();
  }

  private void BeforeFixedUpdate() {
    // Restoring actual transform for the FixedUpdate() cal where it could be change by the user.
    RestoreActualTransform();
  }

  private void AfterFixedUpdate() {
    // Saving actual transform for being restored in the BeforeFixedUpdate() method.
    SaveActualTransform();
  }

  private void LateUpdate() {
    // Set interpolated transform for being rendered.
    SetInterpolatedTransform();
  }

  public void SetPositionImmediate(Vector3 position) {
    _transforms[0].position = position;
    _transforms[1].position = position;
  }

  #region Helpers

  private void RestoreActualTransform() {
    var latest = _transforms[_index];
    transform.localPosition = latest.position;
    transform.localScale = latest.scale;
    transform.localRotation = latest.rotation;
  }

  private void SaveActualTransform() {
    _index = NextIndex();
    _transforms[_index] = CurrentTransformData();
  }

  private void SetInterpolatedTransform() {
    var prev = _transforms[NextIndex()];
    float factor = InterpolationManager.Factor;
    transform.localPosition = Vector3.Lerp(prev.position, transform.localPosition, factor);
    transform.localRotation = Quaternion.Slerp(prev.rotation, transform.localRotation, factor);
    transform.localScale = Vector3.Lerp(prev.scale, transform.localScale, factor);
  }

  public void ResetTransforms() {
    _index = 0;
    var td = CurrentTransformData();
    _transforms = new TransformData[2] { td, td };
  }

  private TransformData CurrentTransformData() {
    return new TransformData(transform.localPosition, transform.localRotation, transform.localScale);
  }

  private int NextIndex() {
    return (_index == 0) ? 1 : 0;
  }

  private IEnumerator WaitForEndOfFrame() {
    while (true) {
      yield return waitForEndOfFrame;
      BeforeFixedUpdate();
    }
  }

  private IEnumerator WaitForFixedUpdate() {
    while (true) {
      yield return waitForFixedUpdate;
      AfterFixedUpdate();
    }
  }

  private struct TransformData {
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public TransformData(Vector3 position, Quaternion rotation, Vector3 scale) {
      this.position = position;
      this.rotation = rotation;
      this.scale = scale;
    }
  }

  #endregion
}
