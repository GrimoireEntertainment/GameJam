using System.Collections.Generic;
using Game.Level;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class EnergyStationBatteryIndicatorsUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Energy station that drives these battery indicators.")]
        private EnergyStation _energyStation;

        [SerializeField, Tooltip("Indicator images, one per battery slot.")]
        private List<Image> _slotImages = new();

        [Header("Colors")]
        [SerializeField, Tooltip("Color used by occupied battery slots.")]
        private Color _filledColor = Color.green;

        [SerializeField, Tooltip("Color used by empty battery slots.")]
        private Color _emptyColor = Color.gray;

        [Header("Billboard")]
        [SerializeField, Tooltip("Camera transform this UI should face. If empty, Camera.main is used.")]
        private Transform _cameraTransform;

        [SerializeField, Tooltip("Use Camera.main when camera transform is not assigned.")]
        private bool _useMainCamera = true;

        private void OnEnable()
        {
            if (_energyStation == null)
            {
                Debug.LogWarning($"{nameof(EnergyStationBatteryIndicatorsUI)} on {name} is missing an energy station.", this);
                return;
            }

            _energyStation.BatterySlotsChanged += OnBatterySlotsChanged;
            UpdateIndicators(_energyStation.OccupiedBatterySlots, _energyStation.MaxBatterySlots);
        }

        private void Start()
        {
            if (_energyStation != null)
            {
                UpdateIndicators(_energyStation.OccupiedBatterySlots, _energyStation.MaxBatterySlots);
            }
        }

        private void LateUpdate()
        {
            Transform cameraTransform = GetCameraTransform();

            if (cameraTransform == null)
            {
                return;
            }

            Vector3 lookDirection = transform.position - cameraTransform.position;

            if (lookDirection.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        private void OnDisable()
        {
            if (_energyStation != null)
            {
                _energyStation.BatterySlotsChanged -= OnBatterySlotsChanged;
            }
        }

        private void OnBatterySlotsChanged(int occupiedSlots, int maxSlots)
        {
            UpdateIndicators(occupiedSlots, maxSlots);
        }

        private void UpdateIndicators(int occupiedSlots, int maxSlots)
        {
            for (int i = 0; i < _slotImages.Count; i++)
            {
                Image slotImage = _slotImages[i];

                if (slotImage == null)
                {
                    continue;
                }

                bool isValidSlot = i < maxSlots;
                slotImage.enabled = isValidSlot;

                if (!isValidSlot)
                {
                    continue;
                }

                slotImage.color = i < occupiedSlots ? _filledColor : _emptyColor;
            }
        }

        private Transform GetCameraTransform()
        {
            if (_cameraTransform != null)
            {
                return _cameraTransform;
            }

            if (!_useMainCamera || Camera.main == null)
            {
                return null;
            }

            return Camera.main.transform;
        }
    }
}
