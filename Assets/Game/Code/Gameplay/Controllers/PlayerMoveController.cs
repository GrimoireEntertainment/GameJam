using Game.Code.Core;
using UnityEngine;

namespace Game.Gameplay.Controllers
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMoveController : MonoBehaviour, IInjectable
    {
        [Header("Move")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 12f;

        [Header("Jump")]
        [SerializeField] private float _jumpForce = 5f;
        [SerializeField] private float _gravity = -20f;

        [Header("Dash")]
        [SerializeField] private float _dashSpeed = 20f;
        [SerializeField] private float _dashDuration = 0.2f;
        [SerializeField] private float _dashCooldown = 1f;

        [Header("Camera")]
        [SerializeField] private Transform _cameraTransform;

        // Properties
        public bool IsMoveActive { get; set; }
        public Vector3 MoveDirection { get; set; }

        // Components
        private CharacterController _characterController;

        // Movement
        private float _verticalVelocity;

        // Dash
        private bool _isDashing;
        private float _dashTimer;
        private float _dashCooldownTimer;
        private Vector3 _dashDirection;

        public void Construct()
        {
            _characterController = GetComponent<CharacterController>();

            if (_cameraTransform == null && Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            UpdateDashCooldown();

            if (_isDashing)
            {
                UpdateDash();
                return;
            }

            UpdateGravity();
            UpdateMove();
        }

        public void Dash()
        {
            if (_isDashing)
            {
                return;
            }

            if (_dashCooldownTimer > 0f)
            {
                return;
            }

            Vector3 move = GetCameraRelativeMove(MoveDirection);

            _dashDirection = move.sqrMagnitude > 0.001f
                ? move.normalized
                : transform.forward;

            _isDashing = true;
            _dashTimer = _dashDuration;
            _dashCooldownTimer = _dashCooldown;

            _verticalVelocity = 0f;
        }

        public void Jump()
        {
            if (!_characterController.isGrounded)
            {
                return;
            }

            _verticalVelocity = _jumpForce;
        }

        private void UpdateMove()
        {
            Vector3 move = IsMoveActive
                ? GetCameraRelativeMove(MoveDirection)
                : Vector3.zero;

            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    _rotationSpeed * Time.deltaTime);
            }

            Vector3 velocity = move * _moveSpeed;
            velocity.y = _verticalVelocity;

            _characterController.Move(velocity * Time.deltaTime);
        }

        private void UpdateGravity()
        {
            if (_characterController.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            _verticalVelocity += _gravity * Time.deltaTime;
        }

        private void UpdateDash()
        {
            _dashTimer -= Time.deltaTime;

            if (_dashTimer <= 0f)
            {
                _isDashing = false;
                return;
            }

            Vector3 velocity = _dashDirection * _dashSpeed;
            velocity.y = 0f;

            _characterController.Move(velocity * Time.deltaTime);
        }

        private void UpdateDashCooldown()
        {
            if (_dashCooldownTimer <= 0f)
            {
                return;
            }

            _dashCooldownTimer -= Time.deltaTime;
        }

        private Vector3 GetCameraRelativeMove(Vector3 input)
        {
            if (_cameraTransform == null)
            {
                return Vector3.zero;
            }

            Vector3 forward = _cameraTransform.forward;
            Vector3 right = _cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 move = right * input.x + forward * input.z;

            return move.sqrMagnitude > 1f
                ? move.normalized
                : move
            ;
        }
    }
}