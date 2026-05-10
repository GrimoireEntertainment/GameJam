using Game.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Utils
{
    public sealed class DebugHotkeys : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private void Update()
        {
            if (Keyboard.current == null || GameSessionController.Instance == null)
            {
                return;
            }

            if (Keyboard.current.f1Key.wasPressedThisFrame)
            {
                GameSessionController.Instance.Win();
            }

            if (Keyboard.current.f2Key.wasPressedThisFrame)
            {
                GameSessionController.Instance.Lose();
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                GameSessionController.Instance.Restart();
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                GameSessionController.Instance.TogglePause();
            }
        }
#endif
    }
}
