using UnityEngine;

namespace Utility {
  public class ExceptionManager : MonoBehaviour {
    void Awake() {
      Application.logMessageReceived += HandleException;
    }

    void HandleException(string condition, string stackTrace, LogType type) {
      if (type == LogType.Exception) {
        Debug.LogError("Exception: " + condition + "\n" + stackTrace);
      }
    }
  }
}