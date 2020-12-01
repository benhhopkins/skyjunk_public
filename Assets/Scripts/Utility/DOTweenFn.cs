using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

namespace Utility {
  public static class DOFn {

    public static EaseFunction Square(int overshoot = 1) {
      return (float time, float duration, float x, float xx) => {
        return Mathf.RoundToInt(time * overshoot / duration) % 2;
      };
    }
  }
}