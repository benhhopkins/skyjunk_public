using UnityEngine;

namespace Utility {

  public class SerializedVariable<T> : ScriptableObject {
    public T DefaultValue;

    private T currentValue;
    public T Value {
      get => currentValue;
      set {
        currentValue = value;
        if (UpdatedCallback != null)
          UpdatedCallback(currentValue);
      }
    }

    public delegate void Updated(T value);
    public event Updated UpdatedCallback;

    private void OnEnable() {
      currentValue = DefaultValue;
    }
  }
}