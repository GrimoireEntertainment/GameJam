using Game.Core;
using UnityEngine;

namespace Game.Code.Core
{
    public sealed class Bootstrap : MonoBehaviour
    {
        [SerializeField] private SceneLoader _sceneLoader;

        private void Awake()
        {
            if (_sceneLoader == null)
            {
                _sceneLoader = FindFirstObjectByType<SceneLoader>();
            }

            if (_sceneLoader == null)
            {
                Debug.LogError("SceneLoader is missing in Boot scene.", this);

                return;
            }

            _sceneLoader.LoadMainMenu();
        }
    }
}
