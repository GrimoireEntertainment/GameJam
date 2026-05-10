using Game.Core;
using UnityEngine;

namespace Game.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        public void Play()
        {
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
