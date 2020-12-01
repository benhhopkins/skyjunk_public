using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace UI {
  [RequireComponent(typeof(CanvasGroup))]
  public class PopUp : MonoBehaviour {
    private CanvasGroup overlayCanvasGroup;
    private Image overlayImage;

    [SerializeField] private TextMeshProUGUI title = null;
    [SerializeField] private HorizontalLayoutGroup centerLayout = null;
    [SerializeField] private HorizontalLayoutGroup bottomLayout = null;

    public delegate void OnPopUpSelection(UIButton button);
    public event OnPopUpSelection OnSelection;

    void OnEnable() {
      overlayCanvasGroup = GetComponent<CanvasGroup>();
      overlayImage = GetComponent<Image>();
      if (overlayImage) {
        overlayImage.rectTransform.anchorMin = new Vector2(0, 0);
        overlayImage.rectTransform.anchorMax = new Vector2(1, 1);
        overlayImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
      }

      foreach (Transform child in centerLayout.transform) {
        var button = child.GetComponent<UIButton>();
        if (button)
          button.PressedEvent.AddListener(OptionSelected);
      }

      foreach (Transform child in bottomLayout.transform) {
        var button = child.GetComponent<UIButton>();
        if (button)
          button.PressedEvent.AddListener(OptionSelected);
      }

      overlayCanvasGroup.alpha = 0;
      overlayCanvasGroup.blocksRaycasts = false;
    }

    public void Show(bool show = true) {
      if (!show)
        Hide();
      else {
        overlayCanvasGroup.blocksRaycasts = true;
        overlayCanvasGroup.DOFade(1, 0.2f).SetUpdate(true);
        centerLayout.spacing = -100;
        DOTween.To(() => centerLayout.spacing,
          x => centerLayout.spacing = x,
          100, 0.5f).SetEase(Ease.OutQuart);
        transform.SetAsLastSibling();
      }
    }

    public void Hide() {
      overlayCanvasGroup.blocksRaycasts = false;
      overlayCanvasGroup.DOFade(0, 0.2f).SetUpdate(true);
    }

    public void SetTitle(string title) {
      this.title.text = title;
    }

    public void AddCenterButton(UIButton button) {
      button.transform.SetParent(centerLayout.transform);
      button.PressedEvent.AddListener(OptionSelected);
    }

    public void AddBottomButton(UIButton button) {
      button.transform.SetParent(bottomLayout.transform);
      button.PressedEvent.AddListener(OptionSelected);
    }

    private void OptionSelected(UIButton button) {
      if (OnSelection != null)
        OnSelection(button);

      var popUpButton = button.GetComponent<PopUpButton>();
      if (popUpButton) {
        if (popUpButton.closesPopUp)
          Hide();
        if (popUpButton.destroysPopUp) {
          Hide();
          Destroy(gameObject, 1);
        }
      }
    }
  }

}