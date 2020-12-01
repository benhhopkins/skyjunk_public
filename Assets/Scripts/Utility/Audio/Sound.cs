using UnityEngine;

namespace Utility {
  public class Sound : PooledObject {
    [HideInInspector]
    public AudioSource source;

    public bool wasLinked = false;
    public GameObject linked;

    void Awake() {
      source = GetComponent<AudioSource>();
    }

    public Sound CreatePooledInstance(SoundClip sc, Vector3 position, GameObject linked = null) {
      Sound s = GetPooledInstance<Sound>();
      s.source.clip = sc.clips[Random.Range(0, sc.clips.Count - 1)];
      s.source.volume = sc.volume;
      s.source.pitch = sc.pitch;
      s.transform.position = position;
      s.source.loop = sc.loop;
      s.linked = linked;
      if (linked)
        s.wasLinked = true;
      else
        s.wasLinked = false;
      s.source.Play();
      return s;
    }

    void Update() {
      if (!source.isPlaying || (wasLinked && !linked))
        ReturnToPool();
    }

    protected override void ReturnedToPool() {
      source.Stop();
    }
  }
}