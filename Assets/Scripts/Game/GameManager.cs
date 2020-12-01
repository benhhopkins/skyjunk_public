using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Utility;
using Cinemachine;
using UI;

public enum GameState {
  InTransition,
  Menu,
  Intro,
  Game,
  Lose,
  Win,
  Paused
}

public class GameManager : MonoBehaviour {

  [SerializeField]
  private Camera baseCamera = null;
  public Camera BaseCamera => baseCamera;

  [SerializeField]
  private Camera uiCamera = null;
  public Camera UICamera => uiCamera;

  [SerializeField]
  private CinemachineFreeLook cmFreeLook = null;
  public CinemachineFreeLook CMFreeLook => cmFreeLook;

  [SerializeField]
  private Canvas uiGfxCanvas = null;
  public Canvas UIGfxCanvas => uiGfxCanvas;

  private GameState gameState;
  public GameState GameState => gameState;

  private static GameManager _instance = null;
  public static GameManager I => _instance;
  void Awake() {
    _instance = this;
  }

  void Start() {
#if UNITY_EDITOR
    if (SceneManager.GetSceneByName("Procedural Map").IsValid()) {
      UIGroupManager.I.SetUIGroupState("Game", true, true);
      gameState = GameState.Game;
      return;
    } else if (SceneManager.GetSceneByName("MainMenu Map").IsValid()) {
      UIGroupManager.I.SetUIGroupState("Menu", true, true);
      gameState = GameState.Menu;
      return;
    }
#endif

    StartCoroutine(RunMenu());
  }

  public void PlayButtonTransition(UIButton button) {
    if (gameState != GameState.InTransition) {
      gameState = GameState.InTransition;
      StartCoroutine(RunGame());
    }
  }

  public void MenuButtonTransition(UIButton button) {
    if (gameState != GameState.InTransition) {
      gameState = GameState.InTransition;
      StartCoroutine(RunMenu());
    }
  }

  private IEnumerator RunGame() {
    Time.timeScale = 1;
    UIGroupManager.I.SetUIGroupState("Loading", true, true);
    yield return new WaitForSeconds(1);
    SceneSwitcher.I.UnloadScenes("MainMenu Map");
    var asyncLoader = SceneSwitcher.I.LoadSceneAsync("Procedural Map", UnityEngine.SceneManagement.LoadSceneMode.Additive, false);
    while (!asyncLoader.isDone)
      yield return null;

    Time.timeScale = 0;
    LevelTracker.I.generator.Generate();
    Time.timeScale = 1;

    yield return new WaitForSeconds(1);
    AudioManager.I.PlayMusic(1);
    UIGroupManager.I.SetUIGroupState("Intro", true, true);
    gameState = GameState.Intro;
    Cursor.lockState = CursorLockMode.None;
    CMFreeLook.enabled = false;
  }

  private IEnumerator RunMenu() {
    Time.timeScale = 1;
    UIGroupManager.I.SetUIGroupState("Loading", true, true);
    yield return new WaitForSeconds(1);
    SceneSwitcher.I.UnloadScenes("Procedural Map");
    var asyncLoader = SceneSwitcher.I.LoadSceneAsync("MainMenu Map", UnityEngine.SceneManagement.LoadSceneMode.Additive, false);
    while (!asyncLoader.isDone)
      yield return null;

    yield return new WaitForSeconds(1);
    AudioManager.I.PlayMusic(0);
    UIGroupManager.I.SetUIGroupState("Menu", true, true);
    gameState = GameState.Menu;
  }

  public void QuitGame(UIButton button) {
    Application.Quit(0);
  }

  // this stops menu input from controlling the character as the game is started/unpaused
  public IEnumerator ChangeStateNextFrame(GameState gameState) {
    this.gameState = GameState.InTransition;
    yield return new WaitForEndOfFrame();
    this.gameState = gameState;
  }

  public void EndIntro() {
    if (gameState == GameState.Intro) {
      Time.timeScale = 1;
      UIGroupManager.I.SetUIGroupState("Game", true, true);
      Cursor.lockState = CursorLockMode.Locked;
      CMFreeLook.enabled = true;

      StartCoroutine(ChangeStateNextFrame(GameState.Game));
    }
  }

  public void LoseGame() {
    if (gameState == GameState.Game) {
      Time.timeScale = 1;
      Cursor.lockState = CursorLockMode.None;
      CMFreeLook.enabled = false;
      StartCoroutine(ChangeStateNextFrame(GameState.Lose));
      UIGroupManager.I.SetUIGroupState("Lose", true, true);
    }
  }

  public void WinGame() {
    if (gameState == GameState.Game) {
      Time.timeScale = 0;
      Cursor.lockState = CursorLockMode.None;
      CMFreeLook.enabled = false;
      gameState = GameState.Win;
      UIGroupManager.I.SetUIGroupState("Win", true, true);
    }
  }

  public void TogglePaused(InputAction.CallbackContext ctx) {
    if (ctx.performed) {
      if (gameState == GameState.Game) {
        Pause();
      } else if (GameState == GameState.Paused) {
        Unpause();
      }
    }
  }

  public void Pause() {
    if (gameState == GameState.Game) {
      Cursor.lockState = CursorLockMode.None;
      gameState = GameState.Paused;
      Time.timeScale = 0;
      UIGroupManager.I.SetUIGroupState("Paused", true, true);
    }
  }

  public void Unpause() {
    if (GameState == GameState.Paused) {
      Cursor.lockState = CursorLockMode.Locked;
      Time.timeScale = 1;
      UIGroupManager.I.SetUIGroupState("Game", true, true);
      StartCoroutine(ChangeStateNextFrame(GameState.Game));
    }
  }
}