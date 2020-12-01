using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Utility {
  public class AudioManager : MonoBehaviour {
    public SoundClip[] sounds;
    public Sound soundPrefab;

    public AudioClip[] music;
    [HideInInspector] public AudioSource musicSource;

    public AudioMixer masterMixer;

    private static AudioManager instance;
    public static AudioManager I {
      get {
        if (instance == null)
          instance = GameObject.FindObjectOfType<AudioManager>();
        return instance;
      }
    }

    void Awake() {
      musicSource = GetComponent<AudioSource>();

      UnityEngine.Assertions.Assert.IsNotNull(soundPrefab);
      UnityEngine.Assertions.Assert.IsNotNull(musicSource);
    }

    public Sound Play(string name, Vector3 position, GameObject linked = null) {
      SoundClip sc = Array.Find(sounds, sound => sound.name == name);
      if (sc != null) {
        Sound s = soundPrefab.CreatePooledInstance(sc, position, linked);
        return s;
      }
      return null;
    }

    public Sound Play(SoundClip clip, Vector3 position, GameObject linked = null) {
      Sound s = soundPrefab.CreatePooledInstance(clip, position, linked);
      return s;
    }

    public AudioSource PlayMusic(int index) {
      if (index < music.Length)
        musicSource.clip = music[index];

      musicSource.time = 0;
      musicSource.Play();

      return musicSource;
    }

    public void SetMasterVolume(float value) {
      masterMixer.SetFloat("masterVolume", -80 + value * 100);
    }

    public void SetMusicVolume(float value) {
      masterMixer.SetFloat("musicVolume", -80 + value * 100);
    }

    public void SetSFXVolume(float value) {
      masterMixer.SetFloat("sfxVolume", -80 + value * 100);
    }
  }
}