using System;
using UnityEngine;

namespace Game.Gameplay
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 3;
        [SerializeField] private bool _destroyOnDeath = true;

        public event Action<int, int> HealthChanged;
        public event Action Died;

        public int CurrentHealth { get; private set; }
        public int MaxHealth => _maxHealth;
        public bool IsDead => CurrentHealth <= 0;

        private void Awake()
        {
            ResetHealth();
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0 || IsDead)
            {
                return;
            }

            SetHealth(CurrentHealth - amount);

            if (IsDead)
            {
                Died?.Invoke();

                if (_destroyOnDeath)
                {
                    Destroy(gameObject);
                }
            }
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || IsDead)
            {
                return;
            }

            SetHealth(CurrentHealth + amount);
        }

        public void Kill()
        {
            TakeDamage(CurrentHealth);
        }

        public void ResetHealth()
        {
            SetHealth(_maxHealth);
        }

        private void SetHealth(int value)
        {
            int newHealth = Mathf.Clamp(value, 0, _maxHealth);

            if (CurrentHealth == newHealth)
            {
                return;
            }

            CurrentHealth = newHealth;
            HealthChanged?.Invoke(CurrentHealth, _maxHealth);
        }
    }
}
