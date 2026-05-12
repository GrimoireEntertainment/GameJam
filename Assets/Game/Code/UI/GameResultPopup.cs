using Game.Gameplay;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public sealed class GameResultPopup : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private GameObject _winBackgroundRoot;
        [SerializeField] private GameObject _loseBackgroundRoot;
        [SerializeField] private GameObject _restartButtonRoot;
        [SerializeField] private GameObject _nextLevelButtonRoot;
        [SerializeField] private GameObject _mainMenuButtonRoot;

        public void ShowWin()
        {
            Show("Victory", "You completed the game.");
            SetBackground(isWin: true);
            SetButtons(isWin: true);
        }

        public void ShowLose()
        {
            Show("Defeat", "Try again.");
            SetBackground(isWin: false);
            SetButtons(isWin: false);
        }

        public void Hide()
        {
            if (_root != null)
            {
                _root.SetActive(false);
            }

            SetBackground(isWin: null);
        }

        public void OnRestartClicked()
        {
            GameSessionController.Instance.Restart();
        }

        public void OnMainMenuClicked()
        {
            GameSessionController.Instance.LoadMainMenu();
        }

        public void OnNextLevelClicked()
        {
            GameSessionController.Instance.LoadNextLevel();
        }

        private void Show(string title, string description)
        {
            if (_root != null)
            {
                _root.SetActive(true);
            }

            if (_titleText != null)
            {
                _titleText.text = title;
            }

            if (_descriptionText != null)
            {
                _descriptionText.text = description;
            }
        }

        private void SetButtons(bool isWin)
        {
            if (_restartButtonRoot != null)
            {
                _restartButtonRoot.SetActive(true);
            }

            if (_nextLevelButtonRoot != null)
            {
                _nextLevelButtonRoot.SetActive(isWin);
            }

            if (_mainMenuButtonRoot != null)
            {
                _mainMenuButtonRoot.SetActive(!isWin);
            }
        }

        private void SetBackground(bool? isWin)
        {
            if (_winBackgroundRoot != null)
            {
                _winBackgroundRoot.SetActive(isWin == true);
            }

            if (_loseBackgroundRoot != null)
            {
                _loseBackgroundRoot.SetActive(isWin == false);
            }
        }
    }
}
