using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Slider))]
public class SliderPrefsSaver : MonoBehaviour {

  [SerializeField]
  public SliderStart onStart;

  private Slider slider;

  void Awake() {
    slider = GetComponent<Slider>();
    slider.onValueChanged.AddListener(OnChanged);
    slider.value = PlayerPrefs.GetFloat(name, slider.value);
    if (onStart != null)
      onStart.Invoke(slider.value);
  }

  public void OnChanged(float value) {
    PlayerPrefs.SetFloat(name, value);
    PlayerPrefs.Save();
  }

  [Serializable]
  public class SliderStart : UnityEvent<float> { }
}