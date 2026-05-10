using UnityEngine;

namespace Game.Interaction
{
    [RequireComponent(typeof(Collider))]
    public class PlayerInteractionTrigger : MonoBehaviour
    {
        private PlayerInteractor _interactor;

        private void Awake()
        {
            _interactor = GetComponentInParent<PlayerInteractor>();

            Collider triggerCollider = GetComponent<Collider>();

            if (!triggerCollider.isTrigger)
            {
                Debug.LogWarning($"{nameof(PlayerInteractionTrigger)} on {name} needs its Collider set to Is Trigger.", this);
            }

            if (_interactor == null)
            {
                Debug.LogWarning($"{nameof(PlayerInteractionTrigger)} on {name} could not find a parent {nameof(PlayerInteractor)}.", this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_interactor == null)
            {
                return;
            }

            IInteractable interactable = GetInteractable(other);
            _interactor.RegisterInteractable(interactable);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_interactor == null)
            {
                return;
            }

            IInteractable interactable = GetInteractable(other);
            _interactor.UnregisterInteractable(interactable);
        }

        private static IInteractable GetInteractable(Collider other)
        {
            if (other == null)
            {
                return null;
            }

            MonoBehaviour[] behaviours = other.GetComponentsInParent<MonoBehaviour>();

            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is IInteractable interactable)
                {
                    return interactable;
                }
            }

            return null;
        }
    }
}
