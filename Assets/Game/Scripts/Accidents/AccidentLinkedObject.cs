using Game.Level;
using UnityEngine;

namespace Game.Accidents
{
    public class AccidentLinkedObject : MonoBehaviour
    {
        [Header("Accident")]
        [SerializeField, Tooltip("Accident controller that raises start and resolve events.")]
        private AccidentController _accidentController;

        [SerializeField, Tooltip("Accident type id this object can display, for example HullBreach.")]
        private string _accidentId;

        [SerializeField, Tooltip("Optional unique location id. Leave empty if any free point can be used.")]
        private string _locationId;

        [Header("Target")]
        [SerializeField, Tooltip("Root object shown while the accident is active.")]
        private GameObject _targetRoot;

        [SerializeField, Tooltip("Hide the target when this component starts.")]
        private bool _hideOnStart = true;

        private HullBreachRepairPoint _repairPoint;

        public string AccidentTypeId => _accidentId;
        public string LocationId => _locationId;
        public string ActiveInstanceId { get; private set; }
        public bool IsActive => !string.IsNullOrWhiteSpace(ActiveInstanceId);

        private void Awake()
        {
            _repairPoint = _targetRoot != null ? _targetRoot.GetComponentInChildren<HullBreachRepairPoint>(true) : null;
        }

        private void OnEnable()
        {
            if (_accidentController == null)
            {
                Debug.LogWarning($"{nameof(AccidentLinkedObject)} on {name} is missing an accident controller.", this);
                return;
            }

            _accidentController.RegisterLinkedObject(this);
        }

        private void Start()
        {
            if (_targetRoot == null)
            {
                Debug.LogWarning($"{nameof(AccidentLinkedObject)} on {name} is missing a target root.", this);
                return;
            }

            if (IsActive)
            {
                return;
            }

            if (_hideOnStart)
            {
                SetTargetActive(false);
            }
        }

        private void OnDisable()
        {
            if (_accidentController != null)
            {
                _accidentController.UnregisterLinkedObject(this);
            }
        }

        public bool Matches(string accidentTypeId, string locationId)
        {
            if (_accidentId != accidentTypeId)
            {
                return false;
            }

            return string.IsNullOrWhiteSpace(locationId) || _locationId == locationId;
        }

        public void Activate(ActiveAccident accident)
        {
            if (accident == null)
            {
                return;
            }

            ActiveInstanceId = accident.InstanceId;
            SetTargetActive(true);
            _repairPoint?.ActivateBreach(accident.InstanceId);
        }

        public void Deactivate(ActiveAccident accident)
        {
            if (accident != null && ActiveInstanceId != accident.InstanceId)
            {
                return;
            }

            ActiveInstanceId = string.Empty;
            _repairPoint?.DeactivateBreach();
            SetTargetActive(false);
        }

        private void SetTargetActive(bool isActive)
        {
            if (_targetRoot != null)
            {
                _targetRoot.SetActive(isActive);
            }
        }
    }
}
