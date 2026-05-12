using UnityEngine;

namespace Game.Code.Gameplay.Cameracripts
{
    public sealed class CameraFollow : MonoBehaviour
    {
        [Header("Mode")]
        [SerializeField] private bool _isStatic = true;

        [Header("Follow")]
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new(0f, 5f, -7f);
        [SerializeField] private float _followSpeed = 10f;
        [SerializeField] private bool _lookAtTarget = true;

        private Vector3 _currentVelocity;

        private void LateUpdate()
        {
            if (_isStatic)
            {
                return;
            }

            if (_target == null)
            {
                return;
            }

            Vector3 followPosition = _target.position;
            Vector3 targetPosition = followPosition + _offset;
            float smoothTime = _followSpeed > 0f ? 1f / _followSpeed : 0f;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref _currentVelocity,
                smoothTime)
            ;

            if (_lookAtTarget) transform.LookAt(followPosition);
        }
    }
}
