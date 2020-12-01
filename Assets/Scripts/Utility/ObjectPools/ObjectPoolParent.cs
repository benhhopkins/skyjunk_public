using System.Collections.Generic;
using UnityEngine;

namespace Utility {
  public class ObjectPoolParent : MonoBehaviour {
    private static ObjectPoolParent instance;
    public static ObjectPoolParent I => instance;

    void Awake() {
      instance = this;
    }
  }
}