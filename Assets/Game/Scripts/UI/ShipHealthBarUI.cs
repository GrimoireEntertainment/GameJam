using Game.Level;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ShipHealthBarUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Health source displayed by this UI.")]
        private ShipHealthController _healthController;

        [SerializeField, Tooltip("Optional slider filled from 0 to 1.")]
        private Slider _slider;

        [SerializeField, Tooltip("Optional image using Filled image type.")]
        private Image _fillImage;

        private void OnEnable()
        {
            if (_healthController == null)
            {
                Debug.LogWarning($"{nameof(ShipHealthBarUI)} on {name} is missing a health controller.", this);
                return;
            }

            _healthController.HealthChanged += OnHealthChanged;
            UpdateView(_healthController.NormalizedHealth);
        }

        private void Start()
        {
            if (_healthController != null)
            {
                UpdateView(_healthController.NormalizedHealth);
            }
        }

        private void OnDisable()
        {
            if (_healthController != null)
            {
                _healthController.HealthChanged -= OnHealthChanged;
            }
        }

        private void OnHealthChanged(float currentHealth, float maxHealth)
        {
            UpdateView(maxHealth > 0f ? currentHealth / maxHealth : 0f);
        }

        private void UpdateView(float normalizedValue)
        {
            if (_slider != null)
            {
                _slider.value = normalizedValue;
            }

            if (_fillImage != null)
            {
                _fillImage.fillAmount = normalizedValue;
            }
        }
    }
}
