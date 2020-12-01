using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utility {
  public class SceneSwitcher : MonoBehaviour {

    private static SceneSwitcher _instance;
    public static SceneSwitcher I {
      get {
        if (_instance == null)
          _instance = GameObject.FindObjectOfType<SceneSwitcher>();
        return _instance;
      }
    }

    void Start() {
      if (_instance != null && _instance != this) {
        GameObject.Destroy(gameObject);
        return;
      }
      _instance = this;
    }

    public Coroutine LoadScene(string name, LoadSceneMode mode, bool setActive) {
      return StartCoroutine(LoadSceneCoroutine(name, mode, setActive));
    }

    public Coroutine UnloadScenes(params string[] unloadScenes) {
      return StartCoroutine(UnloadScenesCoroutine(unloadScenes));
    }

    private IEnumerator LoadSceneCoroutine(string name, LoadSceneMode mode, bool setActive) {
      Scene loadedScene = SceneManager.GetSceneByName(name);
      if (loadedScene != null && loadedScene.isLoaded) {
        if (setActive)
          SceneManager.SetActiveScene(loadedScene);
        yield break;
      }

      yield return SceneManager.LoadSceneAsync(name, mode);
      if (setActive) {
        yield return null;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
      }
    }

    private IEnumerator UnloadScenesCoroutine(params string[] unloadScenes) {
      foreach (var sceneName in unloadScenes) {
        Scene unloadScene = SceneManager.GetSceneByName(sceneName);
        if (unloadScene != null && unloadScene.isLoaded)
          yield return SceneManager.UnloadSceneAsync(unloadScene);
      }
    }

    public AsyncOperation LoadSceneAsync(string name, LoadSceneMode mode, bool setActive) {
      Scene loadedScene = SceneManager.GetSceneByName(name);
      if (loadedScene != null && loadedScene.isLoaded) {
        if (setActive)
          SceneManager.SetActiveScene(loadedScene);
        return new AsyncOperation();
      }

      var asyncOperation = SceneManager.LoadSceneAsync(name, mode);
      return asyncOperation;
    }
  }
}