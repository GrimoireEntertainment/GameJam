using Game.Gameplay;
using UnityEngine;

namespace Game.UI
{
    public sealed class PausePopup : MonoBehaviour
    {
        [SerializeField] private GameObject _root;

        public void Show()
        {
            if (_root != null)
            {
                _root.SetActive(true);
            }
        }

        public void Hide()
        {
            if (_root != null)
            {
                _root.SetActive(false);
            }
        }

        public void OnResumeClicked()
        {
            GameSessionController.Instance.Resume();
        }

        public void OnRestartClicked()
        {
            GameSessionController.Instance.Restart();
        }

        public void OnMainMenuClicked()
        {
            GameSessionController.Instance.LoadMainMenu();
        }
    }
}
