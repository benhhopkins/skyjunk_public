using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI {

  public class UIHider : MonoBehaviour {

    private RectTransform rt = null;
    public RectTransform RT => rt;

    private bool init = false;
    private CanvasGroup canvasGroup = null;
    private Vector2 homePosition;
    private float disablingTime;

    public Image panelImage;
    private Material instancedMaterial;

    public enum Move {
      SlideUp,
      SlideDown,
      SlideLeft,
      SlideRight
    }
    public Move moveInType = Move.SlideUp;
    public Move moveOutType = Move.SlideDown;
    public float distance = 300;
    public float animationTime = .3f;
    public float delay = 0f;
    public bool noShowOnEnable = false;

    public bool Visible { get; private set; } = false;

    void Awake() {
      Init();
    }

    private void Init() {
      if (init)
        return;
      init = true;
      rt = GetComponent<RectTransform>();
      canvasGroup = GetComponent<CanvasGroup>();
      homePosition = rt.anchoredPosition;

      if (panelImage) {
        instancedMaterial = Instantiate(panelImage.material);
        panelImage.material = instancedMaterial;
      }

      if (gameObject.activeInHierarchy && gameObject.activeSelf)
        Visible = true;
    }

    public void SetVisibility(bool show) {
      Init();
      if (show && !Visible)
        Show();
      else if (!show && Visible)
        Hide();
    }

    public void SetVisibilityInstant(bool show) {
      Init();

      if (show)
        Visible = true;
      else
        Visible = false;

      rt.DOKill();
      Vector2 move;
      if (!show) {
        move = MoveVectors[(int)moveInType];
        move *= distance;
        rt.anchoredPosition = homePosition - move;
      } else {
        move = MoveVectors[(int)moveOutType];
        move *= distance;
        rt.anchoredPosition = homePosition;
      }

      if (canvasGroup == null)
        return;

      canvasGroup.interactable = show;
      canvasGroup.blocksRaycasts = show;

      canvasGroup.DOKill();
      if (!show)
        canvasGroup.alpha = 0;
      else
        canvasGroup.alpha = 1;
    }

    public void SetHomePosition(Vector2 anchoredPosition) {
      Init();
      rt.anchoredPosition = anchoredPosition;
      homePosition = anchoredPosition;
    }

    private void Show() {
      Visible = true;
      disablingTime = 0;

      if (canvasGroup) {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
      }
      gameObject.SetActive(true);
      MoveTween(true, Ease.OutCubic);
      FadeTween(true, Ease.Linear);
    }

    private void Hide() {
      Visible = false;
      disablingTime = delay + animationTime;

      if (canvasGroup) {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
      }
      MoveTween(false, Ease.InCubic);
      FadeTween(false, Ease.Linear);
    }

    protected virtual void Update() {
      if (disablingTime > 0) {
        disablingTime -= Time.unscaledDeltaTime;
        if (disablingTime <= 0)
          gameObject.SetActive(false);
      }
    }

    void OnEnable() {
      if (!noShowOnEnable)
        Show();
    }

    private void MoveTween(bool moveIn, Ease ease) {
      rt.DOKill();

      float delayTime = 0;
      Vector2 move;
      if (moveIn) {
        delayTime = delay;
        move = MoveVectors[(int)moveInType];
        move *= distance;
        rt.anchoredPosition = homePosition - move;
      } else {
        move = MoveVectors[(int)moveOutType];
        move *= distance;
      }

      rt.DOAnchorPos(rt.anchoredPosition + move, animationTime, false).
           SetEase(ease).SetUpdate(true).SetDelay(delayTime);
    }

    private void FadeTween(bool fadeIn, Ease ease) {
      if (canvasGroup == null)
        return;

      canvasGroup.DOKill();

      float delayTime = 0;
      if (fadeIn) {
        delayTime = delay;
        canvasGroup.alpha = 0;
      }

      canvasGroup.DOFade(fadeIn ? 1 : 0, animationTime).
                  SetEase(ease).SetUpdate(true).SetDelay(delayTime);

      if (instancedMaterial && fadeIn) {
        instancedMaterial.SetFloat("_AddColorAmount", 1);
        instancedMaterial.DOFloat(0, "_AddColorAmount", 1.5f * animationTime).
          SetEase(Ease.OutCubic).SetUpdate(true).SetDelay(delayTime);
      }
    }

    private static IReadOnlyList<Vector2> MoveVectors = new List<Vector2> {
      Vector2.up,
      Vector2.down,
      Vector2.left,
      Vector2.right
      }.AsReadOnly();
  }
}