using System;
using Game.Gameplay;
using UnityEngine;

namespace Game.Level
{
    public class LevelProgressController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Optional energy source. Progress pauses when this has no energy.")]
        private EnergyController _energyController;

        [Header("Progress")]
        [SerializeField, Tooltip("Seconds needed to complete the level.")]
        private float _durationToComplete = 180f;

        private float _currentProgress;
        private bool _isCompleted;

        public float CurrentProgress => _currentProgress;
        public float NormalizedProgress => _currentProgress;
        public bool IsCompleted => _isCompleted;

        public event Action<float> ProgressChanged;
        public event Action LevelCompleted;

        private void Update()
        {
            if (_isCompleted || _durationToComplete <= 0f)
            {
                return;
            }

            if (_energyController != null && !_energyController.HasEnergy)
            {
                return;
            }

            SetProgress(_currentProgress + Time.deltaTime / _durationToComplete);
        }

        private void SetProgress(float value)
        {
            float previousProgress = _currentProgress;
            _currentProgress = Mathf.Clamp01(value);

            if (!Mathf.Approximately(previousProgress, _currentProgress))
            {
                ProgressChanged?.Invoke(_currentProgress);
            }

            if (_currentProgress >= 1f && !_isCompleted)
            {
                _isCompleted = true;
                LevelCompleted?.Invoke();
                GameSessionController.Instance?.Win();
            }
        }
    }
}
