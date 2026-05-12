using Game.Core;
using Game.Level;
using UnityEngine;

namespace Game.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        public void Play()
        {
            if (LevelSequenceController.Instance != null)
            {
                LevelSequenceController.Instance.StartSequence();
                return;
            }

            SceneLoader.Instance.LoadGameAsync();
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
