using UnityEngine;
using TMPro;

namespace Utility {
  public class SVIntUpdater : MonoBehaviour {
    public TextMeshProUGUI updateText;
    public IntVariable variable;
    public string stringFormat = "";

    void Awake() {
      variable.UpdatedCallback += Updated;
    }

    void OnDestroy() {
      variable.UpdatedCallback -= Updated;
    }

    private void Updated(int value) {
      if (!updateText)
        Debug.LogWarning("Updater " + gameObject.name + " text not found.");
      else
        updateText.text = value.ToString(stringFormat);
    }
  }
}