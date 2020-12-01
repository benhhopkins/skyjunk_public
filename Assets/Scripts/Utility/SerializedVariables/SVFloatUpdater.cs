using UnityEngine;
using TMPro;

namespace Utility {
  public class SVFloatUpdater : MonoBehaviour {
    public TextMeshProUGUI updateText;
    public FloatVariable variable;
    public string stringFormat = "";

    void Awake() {
      variable.UpdatedCallback += Updated;
    }

    void OnDestroy() {
      variable.UpdatedCallback -= Updated;
    }

    private void Updated(float value) {
      if (!updateText)
        Debug.LogWarning("Updater " + gameObject.name + " text not found.");
      else
        updateText.text = value.ToString(stringFormat);
    }
  }
}