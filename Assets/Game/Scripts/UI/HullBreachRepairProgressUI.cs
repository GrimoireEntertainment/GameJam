using Game.Level;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class HullBreachRepairProgressUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Tooltip("Repair point that drives this progress UI.")]
        private HullBreachRepairPoint _repairPoint;

        [SerializeField, Tooltip("Root object shown only while repair is in progress.")]
        private GameObject _root;

        [SerializeField, Tooltip("Optional slider filled from 0 to 1.")]
        private Slider _slider;

        [SerializeField, Tooltip("Optional image using Filled image type.")]
        private Image _fillImage;

        [Header("Billboard")]
        [SerializeField, Tooltip("Camera transform this UI should face. If empty, Camera.main is used.")]
        private Transform _cameraTransform;

        [SerializeField, Tooltip("Use Camera.main when camera transform is not assigned.")]
        private bool _useMainCamera = true;

        private CanvasGroup _canvasGroup;
        private bool _isVisible;

        private void Awake()
        {
            GameObject root = GetRoot();
            _canvasGroup = root != null ? root.GetComponent<CanvasGroup>() : GetComponent<CanvasGroup>();

            if (_canvasGroup == null && root == gameObject)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void OnEnable()
        {
            if (_repairPoint == null)
            {
                Debug.LogWarning($"{nameof(HullBreachRepairProgressUI)} on {name} is missing a repair point.", this);
                SetVisible(false);
                return;
            }

            _repairPoint.RepairProgressChanged += OnRepairProgressChanged;
            _repairPoint.RepairStateChanged += OnRepairStateChanged;
            UpdateView(_repairPoint.NormalizedRepairProgress);
            SetVisible(false);
        }

        private void Start()
        {
            if (_repairPoint != null)
            {
                UpdateView(_repairPoint.NormalizedRepairProgress);
                SetVisible(false);
            }
        }

        private void LateUpdate()
        {
            if (!_isVisible)
            {
                return;
            }

            Transform cameraTransform = GetCameraTransform();

            if (cameraTransform == null)
            {
                return;
            }

            Transform rootTransform = GetRoot().transform;
            Vector3 lookDirection = rootTransform.position - cameraTransform.position;

            if (lookDirection.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            rootTransform.rotation = Quaternion.LookRotation(lookDirection);
        }

        private void OnDisable()
        {
            if (_repairPoint != null)
            {
                _repairPoint.RepairProgressChanged -= OnRepairProgressChanged;
                _repairPoint.RepairStateChanged -= OnRepairStateChanged;
            }
        }

        private void OnRepairProgressChanged(float normalizedProgress)
        {
            UpdateView(normalizedProgress);
        }

        private void OnRepairStateChanged(bool isRepairing)
        {
            SetVisible(isRepairing);
        }

        private void UpdateView(float normalizedProgress)
        {
            float clampedProgress = Mathf.Clamp01(normalizedProgress);

            if (_slider != null)
            {
                _slider.value = clampedProgress;
            }

            if (_fillImage != null)
            {
                _fillImage.fillAmount = clampedProgress;
            }
        }

        private void SetVisible(bool isVisible)
        {
            _isVisible = isVisible;

            GameObject root = GetRoot();

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = isVisible ? 1f : 0f;
                _canvasGroup.interactable = isVisible;
                _canvasGroup.blocksRaycasts = isVisible;
                return;
            }

            if (root != null && root != gameObject)
            {
                root.SetActive(isVisible);
            }
        }

        private GameObject GetRoot()
        {
            return _root != null ? _root : gameObject;
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
