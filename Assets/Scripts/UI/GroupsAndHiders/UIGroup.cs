using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI {

  [RequireComponent(typeof(RectTransform))]
  public class UIGroup : MonoBehaviour {

    private bool init = false;

    public List<UIHider> uiHiders = new List<UIHider>();
    public enum StateChange {
      NoChange,
      Hidden,
      Visible,
      Show,
      Hide
    }
    [SerializeField] private StateChange startState = StateChange.NoChange;
    public bool Visible { get; private set; }

    public string groupName = "";

    public bool setAsLastSibling = false;

    void Awake() {
      Init();
    }

    void OnDestroy() {
      UIGroupManager.I.GroupChangeEvent -= VisbilityModeChange;
    }

    private void Init() {
      if (init)
        return;
      init = true;
      Visible = gameObject.activeSelf;
      SetVisibility(startState);

      UIGroupManager.I.GroupChangeEvent += VisbilityModeChange;
    }

    public void Show(bool show = true) {
      if (show)
        SetVisibility(StateChange.Show);
      else
        Hide();

      if (setAsLastSibling)
        transform.SetAsLastSibling();
    }

    public void Hide() {
      SetVisibility(StateChange.Hide);
    }

    public void SetVisibility(StateChange state) {
      Init();

      if (state == StateChange.NoChange) {
        Visible = gameObject.activeSelf;
        return;
      }

      if ((Visible && (state == StateChange.Hidden || state == StateChange.Hide)) ||
          (!Visible && (state == StateChange.Visible || state == StateChange.Show))) {
        if (state == StateChange.Hidden) {
          Visible = false;
          gameObject.SetActive(false);
          uiHiders.RemoveAll(h => !h);
          foreach (var hider in uiHiders) {
            hider.SetVisibilityInstant(false);
            hider.gameObject.SetActive(false);
          }
        } else if (state == StateChange.Visible) {
          Visible = true;
          gameObject.SetActive(true);
          uiHiders.RemoveAll(h => !h);
          foreach (var hider in uiHiders) {
            hider.gameObject.SetActive(true);
            hider.SetVisibilityInstant(true);
          }
        } else if (state == StateChange.Show) {
          Visible = true;
          gameObject.SetActive(true);
          uiHiders.RemoveAll(h => !h);
          foreach (var hider in uiHiders)
            hider.SetVisibility(true);
        } else if (state == StateChange.Hide) {
          Visible = false;
          uiHiders.RemoveAll(h => !h);
          foreach (var hider in uiHiders)
            hider.SetVisibility(false);
        }
      }
    }

    private void VisbilityModeChange(string name, bool show, bool hideIfFalse) {
      if (name == groupName) {
        Show(show);
      } else if (hideIfFalse)
        Hide();
    }
  }

}