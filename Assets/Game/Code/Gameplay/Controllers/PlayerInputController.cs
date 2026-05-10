using Game.Code.Core;
using Game.Interaction;
using Game.Items;
using UnityEngine;

namespace Game.Gameplay.Controllers
{
    public sealed class PlayerInputController : MonoBehaviour, IInjectable
    {
        // Components
        private InputMap _inputMap;
        private InputHandler _inputHandler;
        private PlayerMoveController _moveController;
        private PlayerInteractor _interactor;
        private PlayerItemHolder _itemHolder;

        public void Construct()
        {
            // Set components
            _inputMap = new InputMap();
            _inputHandler = new InputHandler(_inputMap);

            _moveController = GetComponentInChildren<PlayerMoveController>();
            _interactor = GetComponentInChildren<PlayerInteractor>();
            _itemHolder = GetComponentInChildren<PlayerItemHolder>();

            // Subscribe to input events
            _inputHandler.OnMoveActive += SetPlayerMoveActive;
            _inputHandler.OnMoveDirection += SetPlayerMoveDirection;
            _inputHandler.OnDash += OnPlayerDash;
            _inputHandler.OnJump += OnPlayerJump;
            _inputHandler.OnThrow += OnPlayerThrow;
            _inputHandler.OnInteractActive += SetPlayerInteractActive;

            // Enable input
            _inputMap.Enable();
        }

        private void SetPlayerMoveActive(bool isActive)
        {
            _moveController.IsMoveActive = isActive;
        }

        private void SetPlayerMoveDirection(Vector3 direction)
        {
            _moveController.MoveDirection = direction;
        }

        private void OnPlayerDash()
        {
            _moveController.Dash();
        }

        private void OnPlayerJump()
        {
            _moveController.Jump();
        }

        private void OnPlayerThrow()
        {
            if (_itemHolder.HasItem)            
            {
                _itemHolder.ThrowItem();
            }
        }

        private void SetPlayerInteractActive(bool isActive)
        {
            if (isActive) _interactor.InteractStarted();
            else _interactor.InteractCanceled();
        }
    }
}