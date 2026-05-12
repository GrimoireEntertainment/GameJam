using System;
using System.Collections.Generic;
using Game.Code.Core;
using Game.Items;
using UnityEngine;

namespace Game.Interaction
{
    public sealed class PlayerInteractor : MonoBehaviour, IInjectable
    {
        [Header("References")]
        [SerializeField]
        private PlayerItemHolder _itemHolder;

        [SerializeField]
        private Transform _interactionPoint;

        private readonly List<IInteractable> _nearbyInteractables = new();
        private readonly Dictionary<IInteractable, int> _nearbyInteractableCounts = new();

        private IInteractable _currentInteractable;

        private bool _isHoldingInteract;
        private float _holdTimer;

        public PlayerItemHolder ItemHolder => _itemHolder;

        public event Action<IInteractable> Interacted;

        public void Construct()
        {
        }

        private void Update()
        {
            RefreshCurrentInteractable();

            UpdateHoldInteract();
        }

        public void InteractStarted()
        {
            if (_currentInteractable == null)
            {
                return;
            }

            // Hold interaction
            if (_currentInteractable is IHoldInteractable holdInteractable)
            {
                if (holdInteractable.CanHoldInteract(this))
                {
                    _isHoldingInteract = true;
                    _holdTimer = 0f;

                    holdInteractable.StartHold(this);

                    return;
                }
            }

            // Instant interaction
            IInteractable interacted = _currentInteractable;
            _currentInteractable.Interact(this);
            Interacted?.Invoke(interacted);

            RefreshCurrentInteractable();
        }

        public void InteractCanceled()
        {
            CancelCurrentHold();
        }

        public void RegisterInteractable(IInteractable interactable)
        {
            if (interactable == null)
            {
                return;
            }

            if (_nearbyInteractableCounts.TryGetValue(interactable, out int count))
            {
                _nearbyInteractableCounts[interactable] = count + 1;
            }
            else
            {
                _nearbyInteractableCounts.Add(interactable, 1);
                _nearbyInteractables.Add(interactable);
            }

            RefreshCurrentInteractable();
        }

        public void UnregisterInteractable(IInteractable interactable)
        {
            if (interactable == null)
            {
                return;
            }

            if (_nearbyInteractableCounts.TryGetValue(interactable, out int count) && count > 1)
            {
                _nearbyInteractableCounts[interactable] = count - 1;
                RefreshCurrentInteractable();
                return;
            }

            _nearbyInteractableCounts.Remove(interactable);
            _nearbyInteractables.Remove(interactable);

            if (_currentInteractable == interactable)
            {
                CancelCurrentHold();

                SetCurrentInteractable(null);
            }

            RefreshCurrentInteractable();
        }

        private void UpdateHoldInteract()
        {
            if (!_isHoldingInteract)
            {
                return;
            }

            if (_currentInteractable is not IHoldInteractable holdInteractable)
            {
                CancelCurrentHold();
                return;
            }

            _holdTimer += Time.deltaTime;

            float progress =
                Mathf.Clamp01(_holdTimer / holdInteractable.HoldDuration);

            holdInteractable.ProcessHold(this, progress);

            if (_holdTimer < holdInteractable.HoldDuration)
            {
                return;
            }

            _isHoldingInteract = false;

            IInteractable interacted = _currentInteractable;
            holdInteractable.CompleteHold(this);
            Interacted?.Invoke(interacted);

            RefreshCurrentInteractable();
        }

        private void CancelCurrentHold()
        {
            if (!_isHoldingInteract)
            {
                return;
            }

            _isHoldingInteract = false;

            if (_currentInteractable is IHoldInteractable holdInteractable)
            {
                holdInteractable.CancelHold(this);
            }
        }

        private void RefreshCurrentInteractable()
        {
            if (_interactionPoint == null)
            {
                return;
            }

            IInteractable closest = null;

            float closestSqrDistance = float.PositiveInfinity;

            for (int i = _nearbyInteractables.Count - 1; i >= 0; i--)
            {
                IInteractable interactable = _nearbyInteractables[i];

                if (interactable == null)
                {
                    _nearbyInteractables.RemoveAt(i);
                    continue;
                }

                if (interactable is not Component component || component == null)
                {
                    _nearbyInteractables.RemoveAt(i);
                    _nearbyInteractableCounts.Remove(interactable);
                    continue;
                }

                float sqrDistance =
                    (component.transform.position - _interactionPoint.position).sqrMagnitude;

                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closest = interactable;
                }
            }

            SetCurrentInteractable(closest);
        }

        private void SetCurrentInteractable(IInteractable interactable)
        {
            if (_currentInteractable == interactable)
            {
                return;
            }

            CancelCurrentHold();

            _currentInteractable?.SetInteractActive(false);

            _currentInteractable = interactable;

            _currentInteractable?.SetInteractActive(true);
        }
    }
}
