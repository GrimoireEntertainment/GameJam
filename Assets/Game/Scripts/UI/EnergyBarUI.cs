using Game.Level;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class EnergyBarUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Energy source displayed by this UI.")]
        private EnergyController _energyController;

        [SerializeField, Tooltip("Optional slider filled from 0 to 1.")]
        private Slider _slider;

        [SerializeField, Tooltip("Optional image using Filled image type.")]
        private Image _fillImage;

        private void OnEnable()
        {
            if (_energyController == null)
            {
                Debug.LogWarning($"{nameof(EnergyBarUI)} on {name} is missing an energy controller.", this);
                return;
            }

            _energyController.EnergyChanged += OnEnergyChanged;
            UpdateView(_energyController.NormalizedEnergy);
        }

        private void Start()
        {
            if (_energyController != null)
            {
                UpdateView(_energyController.NormalizedEnergy);
            }
        }

        private void OnDisable()
        {
            if (_energyController != null)
            {
                _energyController.EnergyChanged -= OnEnergyChanged;
            }
        }

        private void OnEnergyChanged(float currentEnergy, float maxEnergy)
        {
            UpdateView(maxEnergy > 0f ? currentEnergy / maxEnergy : 0f);
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
