using System.Linq;
using UnityEngine;

namespace Game.Code.Core
{
    public sealed class GameplayBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            foreach (var injectable in FindObjectsOfType<MonoBehaviour>(true).OfType<IInjectable>())
            {
                injectable.Construct();
            }

            Debug.Log("GameplayBootstrap: All injectables constructed.");
        }
    }

    public interface IInjectable
    {
        public void Construct();
    }
}
