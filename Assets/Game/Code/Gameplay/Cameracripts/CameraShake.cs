using Game.Audio;
using Game.Accidents;
using Game.Core;
using Game.Level;
using UnityEngine;

namespace Game.Code.Gameplay.Cameracripts
{
    public sealed class CameraShake : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ShipHealthController _shipHealthController;
        [SerializeField] private AccidentController _accidentController;

        [Header("Idle Sway")]
        [SerializeField] private float _idlePositionStrength = 0.025f;
        [SerializeField] private float _idleRotationStrength = 0.2f;
        [SerializeField] private float _idleSpeed = 0.8f;

        [Header("Health Shake")]
        [SerializeField] private float _lowHealthPositionStrength = 0.12f;
        [SerializeField] private float _lowHealthRotationStrength = 1.2f;
        [SerializeField] private float _lowHealthSpeed = 15f;

        [Header("Impulse Shake")]
        [SerializeField] private float _smallDuration = 0.12f;
        [SerializeField] private float _smallStrength = 0.08f;
        [SerializeField] private float _mediumDuration = 0.2f;
        [SerializeField] private float _mediumStrength = 0.18f;
        [SerializeField] private float _hullBreachDuration = 0.45f;
        [SerializeField] private float _hullBreachStrength = 0.42f;
        [SerializeField] private string _hullBreachAccidentId = "HullBreach";

        [Header("Audio")]
        [SerializeField] private float _shipShakeSoundMinStrength = 0.25f;

        private Vector3 _baseLocalPosition;
        private Quaternion _baseLocalRotation;
        private float _impulseTimer;
        private float _impulseDuration;
        private float _impulseStrength;
        private bool _isSubscribedToAccidents;
        private bool _isShakeSoundPlaying;
        private int _activeAccidentCount;

        private void Awake()
        {
            _baseLocalPosition = transform.localPosition;
            _baseLocalRotation = transform.localRotation;

            if (_shipHealthController == null)
            {
                _shipHealthController = FindFirstObjectByType<ShipHealthController>();
            }

            if (_accidentController == null)
            {
                _accidentController = FindFirstObjectByType<AccidentController>();
            }
        }

        private void OnEnable()
        {
            SubscribeToAccidents();
        }

        private void Start()
        {
            if (_shipHealthController == null)
            {
                _shipHealthController = FindFirstObjectByType<ShipHealthController>();
            }

            if (_accidentController == null)
            {
                _accidentController = FindFirstObjectByType<AccidentController>();
            }

            SubscribeToAccidents();
        }

        private void OnDisable()
        {
            if (_accidentController != null && _isSubscribedToAccidents)
            {
                _accidentController.AccidentStarted -= OnAccidentStarted;
                _accidentController.AccidentResolved -= OnAccidentResolved;
            }

            _isSubscribedToAccidents = false;
            _activeAccidentCount = 0;
            StopShakeSound();
            transform.localPosition = _baseLocalPosition;
            transform.localRotation = _baseLocalRotation;
        }

        private void LateUpdate()
        {
            float time = Time.time;
            float lowHealthFactor = GetLowHealthFactor();

            Vector3 idleOffset = GetNoiseOffset(time * _idleSpeed, _idlePositionStrength);
            Vector3 healthOffset = GetNoiseOffset(time * _lowHealthSpeed, _lowHealthPositionStrength * lowHealthFactor);
            Vector3 impulseOffset = GetImpulseOffset();

            float idleRotation = Mathf.Sin(time * _idleSpeed * 1.37f) * _idleRotationStrength;
            float healthRotation = Mathf.PerlinNoise(time * _lowHealthSpeed, 24.31f) - 0.5f;
            healthRotation *= _lowHealthRotationStrength * lowHealthFactor;

            transform.localPosition = _baseLocalPosition + idleOffset + healthOffset + impulseOffset;
            transform.localRotation = _baseLocalRotation * Quaternion.Euler(0f, 0f, idleRotation + healthRotation);
            UpdateShakeSound();
        }

        public void Shake(float duration, float strength)
        {
            _impulseDuration = Mathf.Max(0.01f, duration);
            _impulseTimer = _impulseDuration;
            _impulseStrength = Mathf.Max(0f, strength);
        }

        public void ShakeSmall()
        {
            Shake(_smallDuration, _smallStrength);
        }

        public void ShakeMedium()
        {
            Shake(_mediumDuration, _mediumStrength);
        }

        public void ShakeHullBreach()
        {
            Shake(_hullBreachDuration, _hullBreachStrength);
        }

        private void OnAccidentStarted(ActiveAccident accident)
        {
            if (accident == null || accident.Definition == null)
            {
                return;
            }

            _activeAccidentCount++;

            if (accident.TypeId == _hullBreachAccidentId)
            {
                ShakeHullBreach();
            }
        }

        private void OnAccidentResolved(ActiveAccident accident)
        {
            _activeAccidentCount = Mathf.Max(0, _activeAccidentCount - 1);
        }

        private float GetLowHealthFactor()
        {
            if (_shipHealthController == null || _activeAccidentCount <= 0)
            {
                return 0f;
            }

            return Mathf.Clamp01(1f - _shipHealthController.NormalizedHealth);
        }

        private void SubscribeToAccidents()
        {
            if (_accidentController == null || _isSubscribedToAccidents)
            {
                return;
            }

            _accidentController.AccidentStarted += OnAccidentStarted;
            _accidentController.AccidentResolved += OnAccidentResolved;
            _activeAccidentCount = CountActiveAccidents();
            _isSubscribedToAccidents = true;
        }

        private int CountActiveAccidents()
        {
            if (_accidentController == null)
            {
                return 0;
            }

            int count = 0;

            foreach (ActiveAccident accident in _accidentController.ActiveAccidents)
            {
                if (accident != null)
                {
                    count++;
                }
            }

            return count;
        }

        private Vector3 GetImpulseOffset()
        {
            if (_impulseTimer <= 0f)
            {
                return Vector3.zero;
            }

            _impulseTimer -= Time.deltaTime;

            float normalizedTime = Mathf.Clamp01(_impulseTimer / _impulseDuration);
            float strength = _impulseStrength * normalizedTime;

            return Random.insideUnitSphere * strength;
        }

        private void UpdateShakeSound()
        {
            bool shouldPlay = _impulseTimer > 0f && _impulseStrength >= _shipShakeSoundMinStrength;

            if (shouldPlay && !_isShakeSoundPlaying)
            {
                AudioService.Instance?.StartLoop(GameSoundId.ShipShake);
                _isShakeSoundPlaying = true;
                return;
            }

            if (!shouldPlay && _isShakeSoundPlaying)
            {
                StopShakeSound();
            }
        }

        private void StopShakeSound()
        {
            AudioService.Instance?.StopLoop(GameSoundId.ShipShake);
            _isShakeSoundPlaying = false;
        }

        private static Vector3 GetNoiseOffset(float time, float strength)
        {
            if (strength <= 0f)
            {
                return Vector3.zero;
            }

            float x = Mathf.PerlinNoise(time, 0.13f) - 0.5f;
            float y = Mathf.PerlinNoise(8.17f, time) - 0.5f;
            float z = Mathf.PerlinNoise(time, 16.49f) - 0.5f;

            return new Vector3(x, y, z) * strength;
        }
    }
}
