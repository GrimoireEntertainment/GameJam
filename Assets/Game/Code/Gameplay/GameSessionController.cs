using System;
using Game.Core;
using Game.Level;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class GameSessionController : MonoBehaviour
    {
        public static GameSessionController Instance { get; private set; }

        public event Action<GameSessionState> StateChanged;

        public GameSessionState CurrentState { get; private set; } = GameSessionState.None;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            StartGame();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void StartGame()
        {
            Time.timeScale = 1f;
            LockCursorForGameplay();
            SetState(GameSessionState.Playing);
        }

        public void Win()
        {
            if (CurrentState != GameSessionState.Playing)
            {
                return;
            }

            Time.timeScale = 0f;
            UnlockCursorForUi();
            SetState(GameSessionState.Won);
        }

        public void Lose()
        {
            if (CurrentState != GameSessionState.Playing)
            {
                return;
            }

            Time.timeScale = 0f;
            UnlockCursorForUi();
            SetState(GameSessionState.Lost);
        }

        public void Pause()
        {
            if (CurrentState != GameSessionState.Playing)
            {
                return;
            }

            Time.timeScale = 0f;
            UnlockCursorForUi();
            SetState(GameSessionState.Paused);
        }

        public void Resume()
        {
            if (CurrentState != GameSessionState.Paused)
            {
                return;
            }

            Time.timeScale = 1f;
            LockCursorForGameplay();
            SetState(GameSessionState.Playing);
        }

        public void TogglePause()
        {
            if (CurrentState == GameSessionState.Playing)
            {
                Pause();
                return;
            }

            if (CurrentState == GameSessionState.Paused)
            {
                Resume();
            }
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            LockCursorForGameplay();

            if (LevelSequenceController.Instance != null)
            {
                LevelSequenceController.Instance.ReloadCurrentScene();
                return;
            }

            SceneLoader.Instance.ReloadCurrentScene();
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            UnlockCursorForUi();

            if (LevelSequenceController.Instance != null)
            {
                LevelSequenceController.Instance.LoadMainMenu();
                return;
            }

            SceneLoader.Instance.LoadMainMenu();
        }

        public void LoadNextLevel()
        {
            Time.timeScale = 1f;
            LockCursorForGameplay();

            if (LevelSequenceController.Instance != null)
            {
                LevelSequenceController.Instance.LoadNextScene();
                return;
            }

            SceneLoader.Instance.LoadNextScene();
        }

        private void SetState(GameSessionState state)
        {
            if (CurrentState == state)
            {
                return;
            }

            CurrentState = state;
            StateChanged?.Invoke(CurrentState);
        }

        private void LockCursorForGameplay()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void UnlockCursorForUi()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
