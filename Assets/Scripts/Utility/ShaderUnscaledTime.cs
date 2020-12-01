using UnityEngine;
using UnityEngine.Assertions;

namespace Utility {

  public class ShaderUnscaledTime : MonoBehaviour {
    void Update() {
      Shader.SetGlobalFloat("_GameTime", Time.time);
      Shader.SetGlobalFloat("_UnscaledTime", Time.unscaledTime);
    }
  }
}