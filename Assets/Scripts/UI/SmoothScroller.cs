using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace UI {

  [RequireComponent(typeof(ScrollRect))]
  public class SmoothScroller : MonoBehaviour, IScrollHandler, IBeginDragHandler, IDragHandler {

    public float scrollSpeed = 1;
    public float friction = 1;
    public float startPosition = 0;
    public List<Slider> sliders;
    public bool horizontalScroll = false;
    public bool reverseDirection = false;

    private ScrollRect scrollRect;
    private float scrollDestination = 0;
    private float dragStart = 0;
    private float dragStartScroll = 0;

    void Awake() {
      scrollDestination = startPosition;
      scrollRect = GetComponent<ScrollRect>();
    }

    void Update() {
      if (horizontalScroll)
        scrollRect.horizontalNormalizedPosition += Time.unscaledDeltaTime * (scrollDestination - scrollRect.horizontalNormalizedPosition) * (10 * friction);
      else
        scrollRect.verticalNormalizedPosition += Time.unscaledDeltaTime * (scrollDestination - scrollRect.verticalNormalizedPosition) * (10 * friction);

      foreach (var slider in sliders) {
        if (horizontalScroll)
          slider.SetValueWithoutNotify(scrollRect.horizontalNormalizedPosition);
        else
          slider.SetValueWithoutNotify(scrollRect.verticalNormalizedPosition);
      }
    }

    public void OnSliderChanged(float value) {
      scrollDestination = value;
    }

    public void OnScroll(PointerEventData pointerEventData) {
      float direction = 1;
      if (reverseDirection)
        direction = -1;

      scrollDestination += scrollSpeed * pointerEventData.scrollDelta.y * direction;
      scrollDestination = Mathf.Clamp(scrollDestination, 0, 1);
    }

    public void OnBeginDrag(PointerEventData pointerEventData) {
      dragStartScroll = scrollDestination;
      if (horizontalScroll)
        dragStart = pointerEventData.position.x;
      else
        dragStart = pointerEventData.position.y;
    }

    public void OnDrag(PointerEventData pointerEventData) {
      if (horizontalScroll)
        scrollDestination = dragStartScroll + (dragStart - pointerEventData.position.x) / Screen.width;
      else
        scrollDestination = dragStartScroll + (dragStart - pointerEventData.position.y) / Screen.height;
      scrollDestination = Mathf.Clamp(scrollDestination, 0, 1);
    }

  }
}