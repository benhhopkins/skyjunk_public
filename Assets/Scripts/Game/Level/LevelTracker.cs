using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Cinemachine;

public class LevelTracker : MonoBehaviour {

  public Unit player = null;
  public Transform reticule = null;
  public LevelGenerator generator = null;

  public FloatVariable levelTime = null;

  private static LevelTracker _instance = null;
  public static LevelTracker I => _instance;
  void Awake() {
    _instance = this;

    GameManager.I.CMFreeLook.LookAt = reticule;
    GameManager.I.CMFreeLook.Follow = player.transform;
  }

  void Start() {
    levelTime.Value = 0;
  }

  void Update() {
    if (player == null) {
      GameManager.I.LoseGame();
    }

    levelTime.Value += Time.deltaTime;
  }
}