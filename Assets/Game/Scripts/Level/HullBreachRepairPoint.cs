using System;
using Game.Accidents;
using Game.Interaction;
using Game.Items;
using UnityEngine;

namespace Game.Level
{
    public class HullBreachRepairPoint : MonoBehaviour, IInteractable, IHoldInteractable, IAccidentLocation
    {
        [Header("Accident")]
        [SerializeField, Tooltip("Accident type id, for example HullBreach or Fire.")]
        private string _accidentId;

        [SerializeField, Tooltip("Optional unique place id. Leave empty if any free point can be used.")]
        private string _locationId;

        [SerializeField, Tooltip("Controller that starts and resolves this accident.")]
        private AccidentController _accidentController;

        [Header("Repair")]
        [SerializeField, Tooltip("Item required to hold-use this accident point.")]
        private ItemType _requiredItem = ItemType.MagneticPlug;

        [SerializeField, Tooltip("Seconds the player must hold Interact.")]
        private float _repairDuration = 2f;

        [SerializeField, Tooltip("Whether the required item is consumed on completion.")]
        private bool _consumeItemOnRepair = true;

        [Header("Visuals")]
        [SerializeField, Tooltip("Optional visual-only root. Do not assign this GameObject.")]
        private GameObject _visualRoot;

        [SerializeField, Tooltip("Hide visuals and colliders when this accident is inactive.")]
        private bool _hideOnStart = true;

        [SerializeField, Tooltip("Disable colliders while inactive.")]
        private bool _disableCollidersWhenInactive = true;

        private float _repairProgress;

        private string _activeAccidentId;

        private Renderer[] _renderers;
        private Collider[] _colliders;

        private bool _isActive;
        private bool _isRepairing;
        private bool _isRepaired;

        public string AccidentTypeId => _accidentId;
        public string LocationId => _locationId;
        public string ActiveInstanceId => _activeAccidentId;
        public bool IsActive => _isActive;

        public float NormalizedRepairProgress =>
            _repairDuration > 0f
                ? Mathf.Clamp01(_repairProgress / _repairDuration)
                : 1f;

        public float HoldDuration => _repairDuration;

        public event Action<float> RepairProgressChanged;
        public event Action<bool> RepairStateChanged;
        public event Action Repaired;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>(true);
            _colliders = GetComponentsInChildren<Collider>(true);
        }

        private void OnEnable()
        {
            if (_accidentController == null)
            {
                return;
            }

            _accidentController.RegisterRepairPoint(this);
        }

        private void Start()
        {
            if (_hideOnStart && !_isActive)
            {
                SetBreachVisible(false);
            }
        }

        private void OnDisable()
        {
            if (_accidentController != null)
            {
                _accidentController.UnregisterRepairPoint(this);
            }
        }

        public void ActivateBreach()
        {
            Activate(_accidentId);
        }

        public void ActivateBreach(string activeAccidentId)
        {
            Activate(activeAccidentId);
        }

        public void Activate(ActiveAccident accident)
        {
            if (accident == null)
            {
                return;
            }

            Activate(accident.InstanceId);
        }

        public void Activate(string activeAccidentId)
        {
            _activeAccidentId = activeAccidentId;

            _isActive = true;
            _isRepaired = false;

            ResetRepairProgress();

            SetBreachVisible(true);
        }

        public void Deactivate(ActiveAccident accident)
        {
            if (accident != null &&
                _activeAccidentId != accident.InstanceId)
            {
                return;
            }

            DeactivateBreach();
        }

        public void DeactivateBreach()
        {
            _isActive = false;
            SetRepairing(false);

            _activeAccidentId = string.Empty;

            ResetRepairProgress();

            SetBreachVisible(false);
        }

        public bool Matches(string accidentTypeId, string locationId)
        {
            if (_accidentId != accidentTypeId)
            {
                return false;
            }

            return string.IsNullOrWhiteSpace(locationId)
                   || _locationId == locationId;
        }

        public void SetInteractActive(bool isActive)
        {
        }

        public void Interact(PlayerInteractor interactor)
        {
        }

        public bool CanHoldInteract(PlayerInteractor interactor)
        {
            return _isActive
                   && !_isRepaired
                   && interactor != null
                   && interactor.ItemHolder != null
                   && interactor.ItemHolder.CurrentItemType == _requiredItem;
        }

        public void StartHold(PlayerInteractor interactor)
        {
            if (!CanHoldInteract(interactor))
            {
                return;
            }

            SetRepairing(true);

            ResetRepairProgress();
        }

        public void ProcessHold(PlayerInteractor interactor, float progress)
        {
            if (!_isRepairing || !CanHoldInteract(interactor))
            {
                CancelHold(interactor);
                return;
            }

            _repairProgress = progress * _repairDuration;

            RepairProgressChanged?.Invoke(progress);
        }

        public void CompleteHold(PlayerInteractor interactor)
        {
            if (!_isRepairing)
            {
                return;
            }

            if (!CanHoldInteract(interactor))
            {
                CancelHold(interactor);
                return;
            }

            if (_consumeItemOnRepair &&
                !interactor.ItemHolder.TryConsumeCurrentItem(_requiredItem))
            {
                CancelHold(interactor);
                return;
            }

            SetRepairing(false);
            _isRepaired = true;

            _repairProgress = _repairDuration;

            RepairProgressChanged?.Invoke(1f);

            string accidentIdToResolve =
                !string.IsNullOrWhiteSpace(_activeAccidentId)
                    ? _activeAccidentId
                    : _accidentId;

            _accidentController?.ResolveAccident(accidentIdToResolve);

            Repaired?.Invoke();
        }

        public void CancelHold(PlayerInteractor interactor)
        {
            SetRepairing(false);

            ResetRepairProgress();
        }

        private void ResetRepairProgress()
        {
            _repairProgress = 0f;

            RepairProgressChanged?.Invoke(0f);
        }

        private void SetRepairing(bool isRepairing)
        {
            if (_isRepairing == isRepairing)
            {
                return;
            }

            _isRepairing = isRepairing;
            RepairStateChanged?.Invoke(_isRepairing);
        }

        private void SetBreachVisible(bool isVisible)
        {
            if (_visualRoot != null && _visualRoot != gameObject)
            {
                _visualRoot.SetActive(isVisible);
            }

            SetRenderersEnabled(isVisible);

            if (_disableCollidersWhenInactive)
            {
                SetCollidersEnabled(isVisible);
            }
        }

        private void SetRenderersEnabled(bool isEnabled)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] != null)
                {
                    _renderers[i].enabled = isEnabled;
                }
            }
        }

        private void SetCollidersEnabled(bool isEnabled)
        {
            for (int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i] != null)
                {
                    _colliders[i].enabled = isEnabled;
                }
            }
        }
    }
}
