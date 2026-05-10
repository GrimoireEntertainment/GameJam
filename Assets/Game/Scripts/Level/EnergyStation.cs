using System;
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

        [SerializeField, Tooltip("How many batteries fit in this station.")]
        private int _maxBatterySlots = 4;

        [SerializeField, Tooltip("Whether the required item is consumed on recharge.")]
        private bool _consumeItem = true;

        public int MaxBatterySlots => Mathf.Max(1, _maxBatterySlots);
        public int OccupiedBatterySlots => GetOccupiedBatterySlots();
        public int FreeBatterySlots => MaxBatterySlots - OccupiedBatterySlots;
        public bool HasFreeSlot => FreeBatterySlots > 0;

        public event Action<int, int> BatterySlotsChanged;

        private int _lastOccupiedSlots = -1;

        private void OnEnable()
        {
            if (_energyController != null)
            {
                _energyController.EnergyChanged += OnEnergyChanged;
            }

            NotifySlotsChangedIfNeeded(force: true);
        }

        private void Start()
        {
            NotifySlotsChangedIfNeeded(force: true);
        }

        private void OnDisable()
        {
            if (_energyController != null)
            {
                _energyController.EnergyChanged -= OnEnergyChanged;
            }
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

            _energyController.AddEnergy(GetEnergyPerBattery());
            NotifySlotsChangedIfNeeded(force: true);
        }

        private void OnEnergyChanged(float currentEnergy, float maxEnergy)
        {
            NotifySlotsChangedIfNeeded(force: false);
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
