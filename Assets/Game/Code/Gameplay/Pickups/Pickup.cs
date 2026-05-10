using Game.Core;
using Game.Gameplay.Effects;
using UnityEngine;

namespace Game.Gameplay.Pickups
{
    public sealed class Pickup : MonoBehaviour
    {
        [SerializeField] private PickupType _type = PickupType.Score;
        [SerializeField] private int _amount = 1;
        [SerializeField] private string _requiredTag = "Player";
        [SerializeField] private bool _useRequiredTag = true;
        [SerializeField] private GameObject _pickupEffectPrefab;
        [SerializeField] private AudioClip _pickupSound;
        [SerializeField] private bool _destroyAfterPickup = true;

        private void OnTriggerEnter(Collider other)
        {
            if (_useRequiredTag && !other.CompareTag(_requiredTag))
            {
                return;
            }

            ApplyPickup(other.gameObject);
            SpawnEffect.Spawn(_pickupEffectPrefab, transform.position, transform.rotation);
            AudioService.Instance?.PlaySfx(_pickupSound);

            if (_destroyAfterPickup)
            {
                Destroy(gameObject);
            }
        }

        private void ApplyPickup(GameObject target)
        {
            switch (_type)
            {
                case PickupType.Score:
                    ScoreController.Instance?.AddScore(_amount);
                    break;
                case PickupType.Heal:
                    target.GetComponent<Health>()?.Heal(_amount);
                    break;
                case PickupType.Win:
                    GameSessionController.Instance?.Win();
                    break;
                case PickupType.Lose:
                    GameSessionController.Instance?.Lose();
                    break;
            }
        }
    }
}
