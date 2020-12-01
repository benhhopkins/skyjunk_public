using UnityEngine;

public class Debris : MonoBehaviour {
  public float initialScale = 1;
  public float time = 0;

  void Awake() {
    initialScale = transform.localScale.y;
  }

  void Update() {
    time += Time.deltaTime;
    if (time > 30)
      Destroy(gameObject);

    float t = time / 30;
    transform.localScale = new Vector3(
        transform.localScale.x,
        (1 - t * t * t) * initialScale,
        transform.localScale.z);
  }
}