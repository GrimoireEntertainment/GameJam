using System;
using UnityEngine;

public sealed class InputHandler
{
    // Events
    public event Action<bool> OnMoveActive;
    public event Action<Vector3> OnMoveDirection;
    public event Action OnDash;
    public event Action OnJump;
    public event Action OnThrow;
    public event Action<bool> OnInteractActive;

    // Components
    private InputMap _inputMap;

    public InputHandler(InputMap inputMap)
    {
        _inputMap = inputMap;

        PrepareInput();
    }

    private void PrepareInput()
    {
        _inputMap.Player.Move.started += _ => OnMoveActive?.Invoke(true);
        _inputMap.Player.Move.canceled += _ => OnMoveActive?.Invoke(false);
        _inputMap.Player.Move.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();

            OnMoveDirection?.Invoke(new Vector3(input.x, 0f, input.y));   
        };
        _inputMap.Player.Dash.started += _ => OnDash?.Invoke();
        _inputMap.Player.Jump.started += _ => OnJump?.Invoke();
        _inputMap.Player.Throw.started += _ => OnThrow?.Invoke();
        _inputMap.Player.Interact.started += _ => OnInteractActive?.Invoke(true);
        _inputMap.Player.Interact.canceled += _ => OnInteractActive?.Invoke(false);
    }
}