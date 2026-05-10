using System;
using System.Collections.Generic;
using Game.Gameplay;
using UnityEngine;

namespace Game.Level
{
    public class ShipHealthController : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField, Tooltip("Maximum ship health.")]
        private float _maxHealth = 100f;

        [SerializeField, Tooltip("Current ship health at level start.")]
        private float _currentHealth = 100f;

        private readonly Dictionary<string, float> _damageSources = new();
        private bool _hasFailed;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public float NormalizedHealth => _maxHealth > 0f ? _currentHealth / _maxHealth : 0f;

        public event Action<float, float> HealthChanged;
        public event Action ShipFailed;

        private void Awake()
        {
            _maxHealth = Mathf.Max(0f, _maxHealth);
            SetHealth(_currentHealth);
        }

        private void Update()
        {
            if (_currentHealth <= 0f || _damageSources.Count == 0)
            {
                return;
            }

            float damagePerSecond = 0f;

            foreach (float damageSource in _damageSources.Values)
            {
                damagePerSecond += damageSource;
            }

            if (damagePerSecond > 0f)
            {
                SetHealth(_currentHealth - damagePerSecond * Time.deltaTime);
            }
        }

        public void AddDamageSource(string accidentId, float damagePerSecond)
        {
            if (string.IsNullOrWhiteSpace(accidentId) || _damageSources.ContainsKey(accidentId))
            {
                return;
            }

            _damageSources.Add(accidentId, Mathf.Max(0f, damagePerSecond));
        }

        public void RemoveDamageSource(string accidentId)
        {
            if (string.IsNullOrWhiteSpace(accidentId))
            {
                return;
            }

            _damageSources.Remove(accidentId);
        }

        public void Heal(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            SetHealth(_currentHealth + amount);
        }

        public void SetHealth(float value)
        {
            float previousHealth = _currentHealth;
            _currentHealth = Mathf.Clamp(value, 0f, _maxHealth);

            if (!Mathf.Approximately(previousHealth, _currentHealth))
            {
                HealthChanged?.Invoke(_currentHealth, _maxHealth);
            }

            if (_currentHealth <= 0f && !_hasFailed)
            {
                _hasFailed = true;
                ShipFailed?.Invoke();
                GameSessionController.Instance?.Lose();
            }
        }
    }
}
