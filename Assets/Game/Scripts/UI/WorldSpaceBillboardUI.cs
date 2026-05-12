using UnityEngine;

namespace Game.UI
{
    public class WorldSpaceBillboardUI : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField, Tooltip("Camera this UI should face. If empty, Camera.main is used.")]
        private Transform _cameraTransform;

        [SerializeField, Tooltip("Use Camera.main when camera transform is not assigned.")]
        private bool _useMainCamera = true;

        [Header("Rotation")]
        [SerializeField, Tooltip("Keep the UI upright using the camera up direction.")]
        private bool _useCameraUp = true;

        private void LateUpdate()
        {
            Transform cameraTransform = GetCameraTransform();

            if (cameraTransform == null)
            {
                return;
            }

            Vector3 direction = transform.position - cameraTransform.position;

            if (direction.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            transform.rotation = _useCameraUp
                ? Quaternion.LookRotation(direction, cameraTransform.up)
                : Quaternion.LookRotation(direction);
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
