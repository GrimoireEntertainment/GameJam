using System;
using Game.Audio;
using Game.Core;
using Game.Interaction;
using Game.Items;
using UnityEngine;

namespace Game.Level
{
    public class EnergyStation : MonoBehaviour, IInteractable
    {
        [Header("Energy")]
        [SerializeField, Tooltip("Energy controller recharged by this station.")]
        private EnergyController _energyController;

        [SerializeField, Tooltip("Item required in player hand to recharge energy.")]
        private ItemType _requiredItem = ItemType.Battery;

        [Header("Slots")]
        [SerializeField, Tooltip("How many batteries fit in this station.")]
        private int _maxBatterySlots = 4;

        [SerializeField, Tooltip("Energy added by one battery. If 0 or less, uses MaxEnergy / MaxBatterySlots.")]
        private float _energyPerBattery;

        [SerializeField, Tooltip("Whether the required item is consumed on recharge.")]
        private bool _consumeItem = true;

        public int MaxBatterySlots => Mathf.Max(1, _maxBatterySlots);
        public int OccupiedBatterySlots => GetOccupiedBatterySlots();
        public int FreeBatterySlots => MaxBatterySlots - OccupiedBatterySlots;
        public bool HasFreeSlot => FreeBatterySlots > 0;
        public float EnergyPerBattery => GetEnergyPerBattery();

        public event Action<int, int> BatterySlotsChanged;

        private int _lastOccupiedSlots = -1;

        private void OnEnable()
        {
            if (_energyController != null)
            {
                _energyController.EnergyChanged += OnEnergyChanged;
            }

            UpdateWorkingSound();
            NotifySlotsChangedIfNeeded(force: true);
        }

        private void OnValidate()
        {
            _maxBatterySlots = Mathf.Max(1, _maxBatterySlots);
            _energyPerBattery = Mathf.Max(0f, _energyPerBattery);
        }

        private void Start()
        {
            UpdateWorkingSound();
            NotifySlotsChangedIfNeeded(force: true);
        }

        private void OnDisable()
        {
            if (_energyController != null)
            {
                _energyController.EnergyChanged -= OnEnergyChanged;
            }

            AudioService.Instance?.StopLoop(GameSoundId.EnergyStationWorking);
        }

        public void SetInteractActive(bool isActive)
        {
        }

        public void Interact(PlayerInteractor interactor)
        {
            Recharge(interactor);
        }

        public bool CanRecharge(PlayerInteractor interactor)
        {
            if (interactor == null || interactor.ItemHolder == null || _energyController == null)
            {
                return false;
            }

            if (interactor.ItemHolder.CurrentItemType != _requiredItem)
            {
                return false;
            }

            return HasFreeSlot;
        }

        public void Recharge(PlayerInteractor interactor)
        {
            if (!CanRecharge(interactor))
            {
                return;
            }

            if (_consumeItem && !interactor.ItemHolder.TryConsumeCurrentItem(_requiredItem))
            {
                return;
            }

            AudioService.Instance?.PlaySfx(GameSoundId.ItemPlaced);
            _energyController.AddEnergy(GetEnergyPerBattery());
            NotifySlotsChangedIfNeeded(force: true);
        }

        public void RefreshBatterySlots()
        {
            NotifySlotsChangedIfNeeded(force: true);
        }

        private void OnEnergyChanged(float currentEnergy, float maxEnergy)
        {
            UpdateWorkingSound();
            NotifySlotsChangedIfNeeded(force: false);
        }

        private void UpdateWorkingSound()
        {
            if (_energyController != null && _energyController.HasEnergy)
            {
                AudioService.Instance?.StartLoop(GameSoundId.EnergyStationWorking);
                return;
            }

            AudioService.Instance?.StopLoop(GameSoundId.EnergyStationWorking);
        }

        private int GetOccupiedBatterySlots()
        {
            if (_energyController == null || _energyController.MaxEnergy <= 0f || _energyController.CurrentEnergy <= 0f)
            {
                return 0;
            }

            float energyPerBattery = GetEnergyPerBattery();

            if (energyPerBattery <= 0f)
            {
                return 0;
            }

            return Mathf.Clamp(Mathf.CeilToInt(_energyController.CurrentEnergy / energyPerBattery), 0, MaxBatterySlots);
        }

        private float GetEnergyPerBattery()
        {
            if (_energyController == null)
            {
                return 0f;
            }

            if (_energyPerBattery > 0f)
            {
                return _energyPerBattery;
            }

            return _energyController.MaxEnergy / MaxBatterySlots;
        }

        private void NotifySlotsChangedIfNeeded(bool force)
        {
            int occupiedSlots = OccupiedBatterySlots;

            if (!force && _lastOccupiedSlots == occupiedSlots)
            {
                return;
            }

            _lastOccupiedSlots = occupiedSlots;
            BatterySlotsChanged?.Invoke(occupiedSlots, MaxBatterySlots);
        }
    }
}
