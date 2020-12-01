using UnityEngine;

namespace UI {

  public class UIGroupManager {
    private static UIGroupManager instance;
    public static UIGroupManager I {
      get {
        if (instance == null)
          instance = new UIGroupManager();
        return instance;
      }
    }

    public delegate void UIGroupChange(string name, bool show, bool hideIfFalse);
    public event UIGroupChange GroupChangeEvent;

    public void SetUIGroupState(string name, bool show, bool hideIfFalse = true) {
      if (GroupChangeEvent != null)
        GroupChangeEvent(name, show, hideIfFalse);
    }
  }

}