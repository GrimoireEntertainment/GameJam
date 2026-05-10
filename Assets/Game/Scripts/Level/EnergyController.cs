using System;
using UnityEngine;

namespace Game.Level
{
    public class EnergyController : MonoBehaviour
    {
        [Header("Energy")]
        [SerializeField, Tooltip("Maximum available ship energy.")]
        private float _maxEnergy = 100f;

        [SerializeField, Tooltip("Current energy at level start.")]
        private float _currentEnergy = 100f;

        [SerializeField, Tooltip("Passive energy drain per second.")]
        private float _energyDrainPerSecond = 1f;

        private bool _wasEmpty;

        public float CurrentEnergy => _currentEnergy;
        public float MaxEnergy => _maxEnergy;
        public float NormalizedEnergy => _maxEnergy > 0f ? _currentEnergy / _maxEnergy : 0f;
        public bool HasEnergy => _currentEnergy > 0f;

        public event Action<float, float> EnergyChanged;
        public event Action EnergyEmpty;

        private void Awake()
        {
            _maxEnergy = Mathf.Max(0f, _maxEnergy);
            SetEnergy(_currentEnergy);
        }

        private void Update()
        {
            if (_currentEnergy <= 0f || _energyDrainPerSecond <= 0f)
            {
                return;
            }

            SetEnergy(_currentEnergy - _energyDrainPerSecond * Time.deltaTime);
        }

        public void AddEnergy(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            SetEnergy(_currentEnergy + amount);
        }

        public void ConsumeEnergy(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            SetEnergy(_currentEnergy - amount);
        }

        public void SetEnergy(float value)
        {
            float previousEnergy = _currentEnergy;
            _currentEnergy = Mathf.Clamp(value, 0f, _maxEnergy);

            if (!Mathf.Approximately(previousEnergy, _currentEnergy))
            {
                EnergyChanged?.Invoke(_currentEnergy, _maxEnergy);
            }

            if (_currentEnergy <= 0f)
            {
                if (!_wasEmpty)
                {
                    _wasEmpty = true;
                    EnergyEmpty?.Invoke();
                }
            }
            else
            {
                _wasEmpty = false;
            }
        }
    }
}
