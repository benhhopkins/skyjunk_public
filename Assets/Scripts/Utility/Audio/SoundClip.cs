using System.Collections.Generic;
using UnityEngine;

namespace Utility {
  [System.Serializable]
  public class SoundClip {
    public string name;
    public List<AudioClip> clips;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(.1f, 3f)]
    public float pitch = 1f;

    public bool loop = false;
  }

  public static class SoundClipExtensions {
    public static Sound Play(this SoundClip soundClip, Vector3 position, GameObject linked = null) {
      if (soundClip != null && soundClip.clips != null && soundClip.clips.Count > 0)
        return AudioManager.I.Play(soundClip, position, linked);
      return null;
    }
  }
}