using Game.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public sealed class HealthView : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private GameObject _root;
        [SerializeField] private Image _fillImage;
        [SerializeField] private Camera _camera;
        [SerializeField] private bool _faceCamera = true;

        private void Start()
        {
            if (_root == null)
            {
                _root = gameObject;
            }

            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_health == null)
            {
                _health = FindFirstObjectByType<Health>();
            }

            if (_health == null)
            {
                UpdateView(0, 0);
                return;
            }

            _health.HealthChanged += OnHealthChanged;
            UpdateView(_health.CurrentHealth, _health.MaxHealth);
        }

        private void LateUpdate()
        {
            FaceCamera();
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.HealthChanged -= OnHealthChanged;
            }
        }

        private void OnHealthChanged(int currentHealth, int maxHealth)
        {
            UpdateView(currentHealth, maxHealth);
        }

        private void UpdateView(int currentHealth, int maxHealth)
        {
            float normalizedHealth = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;

            if (_fillImage != null)
            {
                _fillImage.fillAmount = normalizedHealth;
            }

            if (_root != null)
            {
                _root.SetActive(currentHealth > 0 && currentHealth < maxHealth);
            }
        }

        private void FaceCamera()
        {
            if (!_faceCamera || _root == null)
            {
                return;
            }

            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_camera == null)
            {
                return;
            }

            Transform rootTransform = _root.transform;
            Vector3 direction = rootTransform.position - _camera.transform.position;

            if (direction.sqrMagnitude <= 0.001f)
            {
                return;
            }

            rootTransform.rotation = Quaternion.LookRotation(direction, _camera.transform.up);
        }
    }
}
