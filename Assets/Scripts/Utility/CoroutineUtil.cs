using System.Collections;
using System.Linq;
using UnityEngine;

namespace Utility {
  public static class CoroutineUtil {
    public static IEnumerator WaitForAll(params Coroutine[] coroutines) {
      for (int i = 0; i < coroutines.Length; i++) {
        yield return coroutines[i];
      }
    }
  }

  public class WaitUntilOne : CustomYieldInstruction {
    private System.Func<bool>[] conditions;

    public override bool keepWaiting {
      get {
        for (int i = 0; i < conditions.Length; i++)
          if (conditions[i].Invoke())
            return false;
        return true;
      }
    }

    public WaitUntilOne(params System.Func<bool>[] conditions) {
      this.conditions = conditions;
    }
  }
}