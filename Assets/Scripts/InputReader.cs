using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// This class is the source of input events
/// </summary>
[CreateAssetMenu(menuName ="Input Reader")]
public class InputReader : ScriptableObject, GameInput.IPlayActions
{
    public UnityEvent<InputAction.CallbackContext> InputEventW;
    public UnityEvent<InputAction.CallbackContext> InputEventA;
    public UnityEvent<InputAction.CallbackContext> InputEventS;
    public UnityEvent<InputAction.CallbackContext> InputEventD;
    public UnityEvent<InputAction.CallbackContext> InputEventUp;
    public UnityEvent<InputAction.CallbackContext> InputEventDown;
    public UnityEvent<InputAction.CallbackContext> InputEventLeft;
    public UnityEvent<InputAction.CallbackContext> InputEventRight;
    public UnityEvent<InputAction.CallbackContext> InputEventLeftClick;

    private GameInput gameInput;

    private void OnEnable()
    {
        if (gameInput == null)
        {
            gameInput = new();
            gameInput.Play.SetCallbacks(this);
        }

        gameInput.Play.Enable();
    }

    private void OnDisable()
    {
        gameInput.Play.Disable();
    }

    public void OnW(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            InputEventW?.Invoke(context);
        }
    }

    public void OnA(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            InputEventA?.Invoke(context);
        }
    }

    public void OnS(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            InputEventS?.Invoke(context);
        }
    }

    public void OnD(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            InputEventD?.Invoke(context);
        }
    }

    public void OnUp(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            InputEventUp?.Invoke(context);
        }
    }

    public void OnLeft(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            InputEventLeft?.Invoke(context);
        }
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            InputEventDown?.Invoke(context);
        }
    }

    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            InputEventRight?.Invoke(context);
        }
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            InputEventLeftClick.Invoke(context);
        }
    }

    public void OnPosition(InputAction.CallbackContext context)
    {
        // Nothing to do
    }
}
