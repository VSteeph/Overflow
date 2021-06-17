using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerInputControl : MonoBehaviour
{
    [Header("Player Debug Input Values")]
    public Vector2 direction;
    public Vector2 look;
    public bool jump;
#if !UNITY_IOS || !UNITY_ANDROID
    public bool cursorLocked = true;
#endif

    public void OnMove(InputValue value)
    {
        direction = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        jump = value.isPressed;
    }



#if !UNITY_IOS || !UNITY_ANDROID
    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
#endif
}
