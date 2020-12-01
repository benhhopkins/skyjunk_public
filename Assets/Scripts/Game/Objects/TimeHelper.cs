using System;
using UnityEngine;
using TMPro;
using Utility;

public class TimeHelper : MonoBehaviour {
  public FloatVariable levelTime = null;
  public TextMeshProUGUI timeText = null;

  void Update() {
    TimeSpan ts = TimeSpan.FromSeconds(levelTime.Value);
    timeText.text = ts.ToString(@"mm\.ss\.fff");
  }
}