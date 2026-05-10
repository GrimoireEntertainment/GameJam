using UnityEngine;

namespace Game.Gameplay
{
    public sealed class TriggerZone : MonoBehaviour
    {
        [SerializeField] private TriggerZoneAction _action;
        [SerializeField] private int _scoreAmount = 1;
        [SerializeField] private int _damageAmount = 1;
        [SerializeField] private string _requiredTag = "Player";
        [SerializeField] private bool _useRequiredTag = true;
        [SerializeField] private bool _triggerOnce = true;

        private bool _wasTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (_triggerOnce && _wasTriggered)
            {
                return;
            }

            if (_useRequiredTag && !other.CompareTag(_requiredTag))
            {
                return;
            }

            _wasTriggered = true;
            Execute(other.gameObject);
        }

        private void Execute(GameObject enteredObject)
        {
            switch (_action)
            {
                case TriggerZoneAction.Win:
                    GameSessionController.Instance?.Win();
                    break;
                case TriggerZoneAction.Lose:
                    GameSessionController.Instance?.Lose();
                    break;
                case TriggerZoneAction.Restart:
                    GameSessionController.Instance?.Restart();
                    break;
                case TriggerZoneAction.MainMenu:
                    GameSessionController.Instance?.LoadMainMenu();
                    break;
                case TriggerZoneAction.AddScore:
                    ScoreController.Instance?.AddScore(_scoreAmount);
                    break;
                case TriggerZoneAction.Damage:
                    enteredObject.GetComponent<Health>()?.TakeDamage(_damageAmount);
                    break;
            }
        }
    }
}
