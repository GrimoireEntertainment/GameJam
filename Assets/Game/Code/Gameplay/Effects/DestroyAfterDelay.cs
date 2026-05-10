using UnityEngine;

namespace Game.Gameplay.Effects
{
    public sealed class DestroyAfterDelay : MonoBehaviour
    {
        [SerializeField] private float _delay = 2f;

        private void Start()
        {
            Destroy(gameObject, _delay);
        }
    }
}
