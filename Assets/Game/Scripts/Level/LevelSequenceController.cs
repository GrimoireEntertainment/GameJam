using System.Collections.Generic;
using Game.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Level
{
    public class LevelSequenceController : MonoBehaviour
    {
        public static LevelSequenceController Instance { get; private set; }

        [Header("Scenes")]
        [SerializeField, Tooltip("Scene names loaded in order. Add intro/info scenes here too.")]
        private List<string> _sceneSequence = new();

        [SerializeField, Tooltip("Scene loaded when leaving the level flow.")]
        private string _mainMenuSceneName = SceneNames.MainMenu;

        private int _currentSceneIndex = -1;

        public int CurrentSceneIndex => _currentSceneIndex;
        public int SceneCount => _sceneSequence.Count;

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

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void StartSequence()
        {
            if (_sceneSequence.Count == 0)
            {
                Debug.LogWarning("LevelSequenceController has no scenes in sequence.", this);
                return;
            }

            _currentSceneIndex = 0;
            LoadSceneAtCurrentIndex();
        }

        public void LoadNextScene()
        {
            if (_sceneSequence.Count == 0)
            {
                SceneLoader.Instance?.LoadMainMenu();
                return;
            }

            SyncIndexWithActiveScene();
            _currentSceneIndex++;

            if (_currentSceneIndex >= _sceneSequence.Count)
            {
                LoadMainMenu();
                return;
            }

            LoadSceneAtCurrentIndex();
        }

        public void ReloadCurrentScene()
        {
            SyncIndexWithActiveScene();

            if (_currentSceneIndex >= 0 && _currentSceneIndex < _sceneSequence.Count)
            {
                LoadSceneAtCurrentIndex();
                return;
            }

            SceneLoader.Instance?.ReloadCurrentScene();
        }

        public void LoadMainMenu()
        {
            _currentSceneIndex = -1;

            if (!string.IsNullOrWhiteSpace(_mainMenuSceneName))
            {
                SceneLoader.Instance?.LoadSceneAsync(_mainMenuSceneName);
                return;
            }

            SceneLoader.Instance?.LoadMainMenu();
        }

        private void LoadSceneAtCurrentIndex()
        {
            string sceneName = _sceneSequence[_currentSceneIndex];

            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogWarning($"Level sequence scene at index {_currentSceneIndex} is empty.", this);
                LoadNextScene();
                return;
            }

            SceneLoader.Instance?.LoadSceneAsync(sceneName);
        }

        private void SyncIndexWithActiveScene()
        {
            string activeSceneName = SceneManager.GetActiveScene().name;

            for (int i = 0; i < _sceneSequence.Count; i++)
            {
                if (_sceneSequence[i] == activeSceneName)
                {
                    _currentSceneIndex = i;
                    return;
                }
            }
        }
    }
}
