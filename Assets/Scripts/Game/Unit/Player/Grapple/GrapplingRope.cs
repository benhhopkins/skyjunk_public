using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(LineRenderer))]
public class GrapplingRope : MonoBehaviour {

  public AnimationCurve effectOverTime = null;
  public AnimationCurve effectOverDistance = null;
  public float effectMultiplier = 3;

  private LineRenderer lineRenderer;
  private float travelTime = 1;
  private float totalTime = 0;
  private Vector3 lengthVector;

  void Awake() {
    lineRenderer = GetComponent<LineRenderer>();
  }

  public void SetTravelTime(float travelTime) {
    this.travelTime = travelTime;
    totalTime = 0;
  }

  // call to update the rope positions each frame,
  //  returns the end of the rope
  public Vector3 UpdatePositions(Vector3 start, Vector3 end) {
    lengthVector = end - start;

    float effectTime = travelTime + 1;
    if (totalTime < effectTime) {
      totalTime += Time.deltaTime;
      Vector3 position = start;
      for (int i = 0; i < lineRenderer.positionCount; i++) {
        float length = ((float)i / (float)lineRenderer.positionCount);
        float travelProgress = Mathf.Min(totalTime / travelTime, 1);
        float effectProgress = totalTime / effectTime;

        position = start + lengthVector * length * travelProgress +
          effectOverTime.Evaluate(effectProgress) * effectOverDistance.Evaluate(length) *
          Mathf.Sin(effectProgress * 2 * Mathf.PI + length * 2 * Mathf.PI) * effectMultiplier *
          Vector3.Cross(Vector3.up, lengthVector);
        lineRenderer.SetPosition(i, position);
      }
      return position;
    } else {
      for (int i = 0; i < lineRenderer.positionCount; i++) {
        float length = ((float)i / (float)lineRenderer.positionCount);
        lineRenderer.SetPosition(i,
          start + lengthVector * length);
      }
    }

    return end;
  }
}
