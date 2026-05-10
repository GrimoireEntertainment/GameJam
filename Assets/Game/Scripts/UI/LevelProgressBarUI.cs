using Game.Level;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class LevelProgressBarUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Progress source displayed by this UI.")]
        private LevelProgressController _progressController;

        [SerializeField, Tooltip("Optional slider filled from 0 to 1.")]
        private Slider _slider;

        [SerializeField, Tooltip("Optional image using Filled image type.")]
        private Image _fillImage;

        private void OnEnable()
        {
            if (_progressController == null)
            {
                Debug.LogWarning($"{nameof(LevelProgressBarUI)} on {name} is missing a progress controller.", this);
                return;
            }

            _progressController.ProgressChanged += OnProgressChanged;
            UpdateView(_progressController.NormalizedProgress);
        }

        private void Start()
        {
            if (_progressController != null)
            {
                UpdateView(_progressController.NormalizedProgress);
            }
        }

        private void OnDisable()
        {
            if (_progressController != null)
            {
                _progressController.ProgressChanged -= OnProgressChanged;
            }
        }

        private void OnProgressChanged(float normalizedProgress)
        {
            UpdateView(normalizedProgress);
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
