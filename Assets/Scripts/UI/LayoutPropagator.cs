using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI {

  [ExecuteInEditMode]
  [RequireComponent(typeof(RectTransform))]
  class LayoutPropagator : UIBehaviour, ILayoutElement {

    public RectTransform childCopySize = null;

    private RectTransform rectTransform;
    private RectTransform rt {
      get {
        if (rectTransform == null)
          rectTransform = GetComponent<RectTransform>();
        return rectTransform;
      }
    }

    protected override void OnEnable() {
      base.OnEnable();
      SetDirty();
    }

    protected override void OnDisable() {
      LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
      base.OnDisable();
    }

    protected override void OnDidApplyAnimationProperties() {
      SetDirty();
    }

    public virtual void CalculateLayoutInputHorizontal() { }
    public virtual void CalculateLayoutInputVertical() { }
    public virtual float minWidth { get { return childCopySize ? LayoutUtility.GetMinWidth(childCopySize) : 0; } }
    public virtual float preferredWidth { get { return childCopySize ? LayoutUtility.GetPreferredWidth(childCopySize) : 0; } }
    public virtual float flexibleWidth { get { return childCopySize ? LayoutUtility.GetFlexibleWidth(childCopySize) : 0; } }
    public virtual float minHeight { get { return childCopySize ? LayoutUtility.GetMinHeight(childCopySize) : 0; } }
    public virtual float preferredHeight { get { return childCopySize ? LayoutUtility.GetPreferredHeight(childCopySize) : 0; } }
    public virtual float flexibleHeight { get { return childCopySize ? LayoutUtility.GetFlexibleHeight(childCopySize) : 0; } }
    public virtual int layoutPriority { get { return 1; } }

    protected virtual void OnTransformChildrenChange() {
      SetDirty();
    }

    protected override void OnRectTransformDimensionsChange() {
      base.OnRectTransformDimensionsChange();
      SetDirty();
    }

    protected void SetDirty() {
      if (!IsActive())
        return;
      LayoutRebuilder.MarkLayoutForRebuild(rt);
    }

#if UNITY_EDITOR
    protected override void OnValidate() {
      SetDirty();
    }
#endif
  }
}