using System.Collections;
using Game.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core
{
    public sealed class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(string sceneName)
        {
            LoadSceneAsync(sceneName);
        }

        public void LoadMainMenu()
        {
            LoadMainMenuAsync();
        }

        public void LoadGame()
        {
            LoadGameAsync();
        }

        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        }

        public void LoadMainMenuAsync()
        {
            LoadSceneAsync(SceneNames.MainMenu);
        }

        public void LoadGameAsync()
        {
            LoadSceneAsync(SceneNames.Game);
        }

        public void ReloadCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadNextScene()
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
            {
                LoadMainMenu();
                return;
            }

            LoadSceneByIndex(nextSceneIndex);
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            LoadingScreen.Instance?.Show();

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            if (operation == null)
            {
                LoadingScreen.Instance?.Hide();
                yield break;
            }

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                LoadingScreen.Instance?.SetProgress(progress);
                yield return null;
            }

            LoadingScreen.Instance?.SetProgress(1f);
            LoadingScreen.Instance?.Hide();
        }

        private void LoadSceneByIndex(int sceneIndex)
        {
            StartCoroutine(LoadSceneByIndexRoutine(sceneIndex));
        }

        private IEnumerator LoadSceneByIndexRoutine(int sceneIndex)
        {
            LoadingScreen.Instance?.Show();

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

            if (operation == null)
            {
                LoadingScreen.Instance?.Hide();
                yield break;
            }

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                LoadingScreen.Instance?.SetProgress(progress);
                yield return null;
            }

            LoadingScreen.Instance?.SetProgress(1f);
            LoadingScreen.Instance?.Hide();
        }
    }
}
