using System;
using Game.Interaction;
using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Level
{
    public class LifeSupportMachine : MonoBehaviour, IInteractable, IHoldInteractable
    {
        [Header("Energy")]
        [SerializeField] private EnergyController _energyController;

        [Header("Resources")]
        [SerializeField] private ItemType _batteryType = ItemType.Battery;
        [SerializeField] private ItemType _gearType = ItemType.Gear;

        [Header("Battery Settings")]
        [SerializeField] private float _batteryEnergy = 35f;
        [SerializeField] private bool _consumeBattery = true;

        [Header("Gear Repair")]
        [SerializeField] private float _repairDuration = 2.5f;
        [SerializeField] private float _gearRepairAmount = 50f;

        [Header("UI")]
        [SerializeField] private Slider _holdProgressSlider;

        [Header("Degradation")]
        [SerializeField] private float _passiveEnergyDrain = 2f;

        public float HoldDuration => _repairDuration;

        private float _repairProgress;
        private bool _isRepairing;

        public event Action<float> RepairProgressChanged;
        public event Action Repaired;

        private void Awake()
        {
            HideSlider();
        }

        private void OnEnable()
        {
            if (_energyController != null)
            {
                _energyController.EnergyChanged += OnEnergyChanged;
            }
            else
            {
                Debug.LogWarning($"{nameof(LifeSupportMachine)} missing EnergyController", this);
            }
        }

        private void OnDisable()
        {
            if (_energyController != null)
            {
                _energyController.EnergyChanged -= OnEnergyChanged;
            }
        }

        private void Update()
        {
            if (_energyController == null)
            {
                return;
            }

            if (_energyController.CurrentEnergy <= 0f)
            {
                _energyController.AddEnergy(-_passiveEnergyDrain * Time.deltaTime);
            }
        }

        private void OnEnergyChanged(float current, float max)
        {
        }

        // -------------------------
        // BATTERY (INSTANT)
        // -------------------------

        public void SetInteractActive(bool isActive)
        {
        }

        public void Interact(PlayerInteractor interactor)
        {
            TryInsertBattery(interactor);
        }

        private void TryInsertBattery(PlayerInteractor interactor)
        {
            if (!CanInsertBattery(interactor))
            {
                return;
            }

            if (_energyController == null)
            {
                Debug.LogWarning($"{nameof(LifeSupportMachine)} missing EnergyController", this);
                return;
            }

            if (_consumeBattery && !interactor.ItemHolder.TryConsumeCurrentItem(_batteryType))
            {
                Debug.LogWarning($"{nameof(LifeSupportMachine)} battery consume failed", this);
                return;
            }

            _energyController.AddEnergy(_batteryEnergy);
        }

        private bool CanInsertBattery(PlayerInteractor interactor)
        {
            if (interactor == null || interactor.ItemHolder == null)
            {
                return false;
            }

            return interactor.ItemHolder.CurrentItemType == _batteryType;
        }

        // -------------------------
        // GEAR (HOLD INTERACTION)
        // -------------------------

        public bool CanHoldInteract(PlayerInteractor interactor)
        {
            if (interactor == null || interactor.ItemHolder == null)
            {
                return false;
            }

            if (_energyController == null)
            {
                return false;
            }

            return interactor.ItemHolder.CurrentItemType == _gearType;
        }

        public void StartHold(PlayerInteractor interactor)
        {
            if (!CanHoldInteract(interactor))
            {
                return;
            }

            _isRepairing = true;
            _repairProgress = 0f;

            ShowSlider();
            SetSliderProgress(0f);
        }

        public void ProcessHold(PlayerInteractor interactor, float progress)
        {
            if (!_isRepairing || !CanHoldInteract(interactor))
            {
                CancelHold(interactor);
                return;
            }

            _repairProgress = progress;

            SetSliderProgress(progress);
            RepairProgressChanged?.Invoke(progress);

            if (progress >= 1f)
            {
                CompleteHold(interactor);
            }
        }

        public void CompleteHold(PlayerInteractor interactor)
        {
            HideSlider();

            if (!CanHoldInteract(interactor))
            {
                CancelHold(interactor);
                return;
            }

            if (!interactor.ItemHolder.TryConsumeCurrentItem(_gearType))
            {
                CancelHold(interactor);
                return;
            }

            if (_energyController == null)
            {
                return;
            }

            _energyController.AddEnergy(_gearRepairAmount);

            _isRepairing = false;
            _repairProgress = 0f;

            RepairProgressChanged?.Invoke(1f);
            Repaired?.Invoke();
        }

        public void CancelHold(PlayerInteractor interactor)
        {
            _isRepairing = false;
            _repairProgress = 0f;

            HideSlider();
            RepairProgressChanged?.Invoke(0f);
        }

        // -------------------------
        // UI (SLIDER)
        // -------------------------

        private void ShowSlider()
        {
            if (_holdProgressSlider == null)
            {
                return;
            }

            _holdProgressSlider.gameObject.SetActive(true);
        }

        private void HideSlider()
        {
            if (_holdProgressSlider == null)
            {
                return;
            }

            _holdProgressSlider.gameObject.SetActive(false);
            _holdProgressSlider.value = 0f;
        }

        private void SetSliderProgress(float progress)
        {
            if (_holdProgressSlider == null)
            {
                return;
            }

            _holdProgressSlider.value = progress;
        }
    }
}