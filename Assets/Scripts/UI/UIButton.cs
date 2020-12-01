using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using DG.Tweening;
using TMPro;
using Utility;
using Sirenix.OdinInspector;

namespace UI {
  public class UIButton : SerializedMonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {
    private RectTransform rectTransform = null;

    public Image image;
    public TextMeshProUGUI text;

    public ColorVariable baseColor;
    public ColorVariable hoverColor;
    public ColorVariable pressedColor;

    public InputAction pressAction;

    private bool pointerHover = false;
    private float yBase = yBaseUnset;
    private Tween colorTweenImage;
    private Tween colorTweenText;
    private Tween positionTween;

    private const float yHover = -3;
    private const float yPress = -6;
    private const float yBaseUnset = -9999;

    [Serializable]
    public class UIButtonPressed : UnityEvent<UIButton> { }
    public UIButtonPressed PressedEvent;

    public bool Selectable { get; set; }
    public bool Selected { get; private set; }

    void Awake() {
      rectTransform = GetComponent<RectTransform>();

      pressAction.performed += ctx => PressedInternal();
    }

    void OnEnable() {
      pressAction.Enable();
    }

    void OnDisable() {
      pressAction.Disable();
    }

    void Start() {
      UpdateHoverColor();
    }

    public void Press() {
      PressedInternal();
    }

    public void SetAnchoredPosition(Vector2 anchoredPosition) {
      rectTransform.anchoredPosition = anchoredPosition;
      yBase = rectTransform.anchoredPosition.y;

      var uiHider = GetComponent<UIHider>();
      if (uiHider)
        uiHider.SetHomePosition(anchoredPosition);
    }

    protected virtual void Pressed() { }

    public virtual void OnPointerDown(PointerEventData pointerEventData) {
      PressedInternal();
    }

    public virtual void OnPointerUp(PointerEventData pointerEventData) {
      PressedInternal();
    }

    public virtual void OnPointerEnter(PointerEventData pointerEventData) {
      pointerHover = true;
      UpdateHoverColor();
      UpdateHoverOffset();
    }

    public virtual void OnPointerExit(PointerEventData pointerEventData) {
      pointerHover = false;
      UpdateHoverColor();
      UpdateHoverOffset();
    }

    private void PressedInternal() {
      if (Selectable) {
        Selected = !Selected;
      }

      if (PressedEvent != null)
        PressedEvent.Invoke(this);

      Pressed();

      SetFlash(Color.white);
      PressEffect();
    }

    private void UpdateHoverColor() {
      if (colorTweenImage != null)
        colorTweenImage.Kill();
      if (colorTweenText != null)
        colorTweenText.Kill();

      Color color = baseColor.Color;
      if (pointerHover)
        color = hoverColor.Color;

      if (image)
        colorTweenImage = image.DOColor(color, 0.15f).SetUpdate(true);
      if (text)
        colorTweenText = text.DOColor(color, 0.15f).SetUpdate(true);

    }

    private void SetFlash(Color color) {
      if (colorTweenImage != null)
        colorTweenImage.Kill();
      if (colorTweenText != null)
        colorTweenText.Kill();

      if (image)
        colorTweenImage = image.DOColor(color, 0.1f).SetUpdate(true).OnComplete(() => {
          if (pointerHover)
            colorTweenImage = image.DOColor(pressedColor.Color, 0.1f).SetUpdate(true);
          else
            colorTweenImage = image.DOColor(baseColor.Color, 0.1f).SetUpdate(true);
        });

      if (text)
        colorTweenText = text.DOColor(color, 0.1f).SetUpdate(true).OnComplete(() => {
          if (pointerHover)
            colorTweenText = text.DOColor(pressedColor.Color, 0.1f).SetUpdate(true);
          else
            colorTweenText = text.DOColor(baseColor.Color, 0.1f).SetUpdate(true);
        });
    }

    private void UpdateHoverOffset() {
      if (yBase == yBaseUnset)
        yBase = rectTransform.anchoredPosition.y;

      if (positionTween != null)
        positionTween.Kill();

      float y = yBase;
      if (pointerHover)
        y += yHover;

      positionTween = rectTransform.DOAnchorPosY(y, 0.15f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void PressEffect() {
      if (yBase == yBaseUnset)
        yBase = rectTransform.anchoredPosition.y;

      if (positionTween != null)
        positionTween.Kill();

      positionTween = rectTransform.DOAnchorPosY(yBase + yHover + yPress, 0.1f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() => {
        UpdateHoverOffset();
      });
    }
  }
}