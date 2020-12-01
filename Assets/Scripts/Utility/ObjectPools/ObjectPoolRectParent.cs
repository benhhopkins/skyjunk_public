using System.Collections.Generic;
using UnityEngine;

namespace Utility {
  public class ObjectPoolRectParent : MonoBehaviour {
    private static ObjectPoolRectParent instance;
    public static ObjectPoolRectParent I => instance;

    void Awake() {
      instance = this;
    }
  }
}