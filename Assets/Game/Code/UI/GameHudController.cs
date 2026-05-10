using Game.Gameplay;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public sealed class GameHudController : MonoBehaviour
    {
        [SerializeField] private GameResultPopup _resultPopup;
        [SerializeField] private PausePopup _pausePopup;

        private GameSessionController _gameSessionController;
        private ScoreController _scoreController;

        private void Start()
        {
            _gameSessionController = GameSessionController.Instance;

            _scoreController = ScoreController.Instance;

            if (_gameSessionController == null)
            {
                return;
            }

            _gameSessionController.StateChanged += OnStateChanged;
            OnStateChanged(_gameSessionController.CurrentState);
        }

        private void OnDisable()
        {
            if (_gameSessionController != null)
            {
                _gameSessionController.StateChanged -= OnStateChanged;
                _gameSessionController = null;
            }
        }

        private void OnStateChanged(GameSessionState state)
        {
            switch (state)
            {
                case GameSessionState.Won:
                    _resultPopup?.ShowWin();
                    _pausePopup?.Hide();
                    break;
                case GameSessionState.Lost:
                    _resultPopup?.ShowLose();
                    _pausePopup?.Hide();
                    break;
                case GameSessionState.Playing:
                    _resultPopup?.Hide();
                    _pausePopup?.Hide();
                    break;
                case GameSessionState.Paused:
                    _pausePopup?.Show();
                    break;
            }
        }
    }
}
