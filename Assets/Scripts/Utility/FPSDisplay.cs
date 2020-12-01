using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour {

  public float updateInterval = 0.25f;

  private float accum = 0;
  private int frames = 0;
  private float timeleft;

  private string text;
  private Color color = Color.white;

  void Start() {
    timeleft = updateInterval;
  }

  void Update() {
    timeleft -= Time.unscaledDeltaTime;
    accum += Time.unscaledDeltaTime;
    ++frames;

    // Interval ended - update GUI text and start new interval
    if (timeleft <= 0.0) {
      // display two fractional digits (f2 format)
      float fps = frames / accum;
      string format = System.String.Format("{0:F2} FPS", fps);
      text = format;

      if (fps < 60)
        color = Color.yellow;
      else
        if (fps < 30)
        color = Color.red;
      else
        color = Color.green;

      timeleft = updateInterval;
      accum = 0;
      frames = 0;
    }
  }

  void OnGUI() {
    int w = Screen.width, h = Screen.height;
    Rect rect = new Rect(0, 0, w, h * 2 / 100);
    GUIStyle style = new GUIStyle();
    style.alignment = TextAnchor.UpperLeft;
    style.fontSize = h * 2 / 100;
    style.normal.textColor = color;
    GUI.Label(rect, text, style);
  }

}