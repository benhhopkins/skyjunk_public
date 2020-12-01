using UnityEngine;
using UnityEngine.UI;

namespace UI {
  public class FittedLayout : LayoutGroup {

    [SerializeField] private RectTransform fitToChild = null;

    [SerializeField] protected bool m_FitWidth = true;
    public bool fitWidth { get { return m_FitWidth; } set { SetProperty(ref m_FitWidth, value); } }

    [SerializeField] protected bool m_FitHeight = true;
    public bool fitHeight { get { return m_FitHeight; } set { SetProperty(ref m_FitHeight, value); } }


    protected override void OnEnable() {
      base.OnEnable();
      Fit();
    }

    public override void SetLayoutHorizontal() {
    }

    public override void SetLayoutVertical() {
    }

    public override void CalculateLayoutInputVertical() {
      Fit();
    }

    public override void CalculateLayoutInputHorizontal() {
      Fit();
    }

#if UNITY_EDITOR
    protected override void OnValidate() {
      base.OnValidate();
      Fit();
    }
#endif

    void Fit() {
      m_Tracker.Clear();
      if (transform.childCount == 0)
        return;

      foreach (RectTransform child in rectTransform) {
        if (child == fitToChild) {
          if (fitWidth) {
            SetLayoutInputForAxis(
              LayoutUtility.GetMinSize(child, 0),
              LayoutUtility.GetPreferredSize(child, 0),
              LayoutUtility.GetFlexibleSize(child, 0),
              0);
          }

          if (fitHeight) {
            SetLayoutInputForAxis(
              LayoutUtility.GetMinSize(child, 1),
              LayoutUtility.GetPreferredSize(child, 1),
              LayoutUtility.GetFlexibleSize(child, 1),
              1);
          }
        }
      }
    }
  }
}
