using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Utility;

namespace UI {
  public abstract class UIToggleIndicator : MonoBehaviour {
    public Image image;
    public ColorVariable activeColor;
    public ColorVariable inactiveColor;

    void Update() {
      if (CheckCondition())
        image.color = activeColor.Color;
      else
        image.color = inactiveColor.Color;
    }

    public abstract bool CheckCondition();
  }
}