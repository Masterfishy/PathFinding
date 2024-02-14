using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// This class is the source of input events
/// </summary>
[CreateAssetMenu(menuName ="Input Reader")]
public class InputReader : ScriptableObject, GameInput.IPlayActions
{
    public UnityEvent<InputAction.CallbackContext> PressEventW;
    public UnityEvent<InputAction.CallbackContext> PressEventA;
    public UnityEvent<InputAction.CallbackContext> PressEventS;
    public UnityEvent<InputAction.CallbackContext> PressEventD;
    public UnityEvent<InputAction.CallbackContext> PressEventUp;
    public UnityEvent<InputAction.CallbackContext> PressEventDown;
    public UnityEvent<InputAction.CallbackContext> PressEventLeft;
    public UnityEvent<InputAction.CallbackContext> PressEventRight;

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
        if (PressEventW != null && (context.performed || context.canceled))
        {
            PressEventW.Invoke(context);
        }
    }

    public void OnA(InputAction.CallbackContext context)
    {
        if (PressEventA != null && (context.performed || context.canceled))
        {
            PressEventA.Invoke(context);
        }
    }

    public void OnS(InputAction.CallbackContext context)
    {
        if (PressEventS != null && (context.performed || context.canceled))
        {
            PressEventS.Invoke(context);
        }
    }

    public void OnD(InputAction.CallbackContext context)
    {
        if (PressEventD != null && (context.performed || context.canceled))
        {
            PressEventD.Invoke(context);
        }
    }

    public void OnUp(InputAction.CallbackContext context)
    {
        if (PressEventUp != null && (context.performed || context.canceled))
        {
            PressEventUp.Invoke(context);
        }
    }

    public void OnLeft(InputAction.CallbackContext context)
    {
        if (PressEventLeft != null && (context.performed || context.canceled))
        {
            PressEventLeft.Invoke(context);
        }
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if (PressEventDown != null && (context.performed || context.canceled))
        {
            PressEventDown.Invoke(context);
        }
    }

    public void OnRight(InputAction.CallbackContext context)
    {
        if (PressEventRight != null && (context.performed || context.canceled))
        {
            PressEventRight.Invoke(context);
        }
    }
}
