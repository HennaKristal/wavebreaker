using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    public Vector2 Move { get; private set; }

    public bool EscapePressed { get; private set; }
    public bool EscapeReleased { get; private set; }
    public bool EscapeHeld { get; private set; }

    public bool ScreenshotPressed { get; private set; }
    public bool ScreenshotReleased { get; private set; }
    public bool ScreenshotHeld { get; private set; }

    public bool MainWeaponPressed { get; private set; }
    public bool MainWeaponReleased { get; private set; }
    public bool MainWeaponHeld { get; private set; }

    public bool EnterPressed { get; private set; }
    public bool EnterReleased { get; private set; }
    public bool EnterHeld { get; private set; }

    public bool CancelPressed { get; private set; }
    public bool CancelReleased { get; private set; }
    public bool CancelHeld { get; private set; }

    private PlayerInputActions PlayerInputActions
    {
        get
        {
            if (playerInputActions == null)
            {
                playerInputActions = new PlayerInputActions();
            }

            return playerInputActions;
        }
    }

    private void OnEnable()
    {
        PlayerInputActions.Enable();
    }

    private void OnDisable()
    {
        PlayerInputActions.Disable();
    }

    private void Update()
    {
        Move = PlayerInputActions.Gameplay.Move.ReadValue<Vector2>();

        EscapePressed = PlayerInputActions.Gameplay.Escape.WasPressedThisFrame();
        EscapeReleased = PlayerInputActions.Gameplay.Escape.WasReleasedThisFrame();
        EscapeHeld = PlayerInputActions.Gameplay.Escape.IsPressed();

        ScreenshotPressed = PlayerInputActions.Gameplay.Screenshot.WasPressedThisFrame();
        ScreenshotReleased = PlayerInputActions.Gameplay.Screenshot.WasReleasedThisFrame();
        ScreenshotHeld = PlayerInputActions.Gameplay.Screenshot.IsPressed();

        MainWeaponPressed = PlayerInputActions.Gameplay.FireMainWeapon.WasPressedThisFrame();
        MainWeaponReleased = PlayerInputActions.Gameplay.FireMainWeapon.WasReleasedThisFrame();
        MainWeaponHeld = PlayerInputActions.Gameplay.FireMainWeapon.IsPressed();

        EnterPressed = PlayerInputActions.Gameplay.Enter.WasPressedThisFrame();
        EnterReleased = PlayerInputActions.Gameplay.Enter.WasReleasedThisFrame();
        EnterHeld = PlayerInputActions.Gameplay.Enter.IsPressed();

        CancelPressed = PlayerInputActions.Gameplay.Cancel.WasPressedThisFrame();
        CancelReleased = PlayerInputActions.Gameplay.Cancel.WasReleasedThisFrame();
        CancelHeld = PlayerInputActions.Gameplay.Cancel.IsPressed();
    }
}
