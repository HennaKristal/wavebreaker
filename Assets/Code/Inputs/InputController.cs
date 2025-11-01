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

    public bool LeftInnerWeaponPressed { get; private set; }
    public bool LeftInnerWeaponReleased { get; private set; }
    public bool LeftInnerWeaponHeld { get; private set; }

    public bool LeftOuterWeaponPressed { get; private set; }
    public bool LeftOuterWeaponReleased { get; private set; }
    public bool LeftOuterWeaponHeld { get; private set; }

    public bool RightInnerWeaponPressed { get; private set; }
    public bool RightInnerWeaponReleased { get; private set; }
    public bool RightInnerWeaponHeld { get; private set; }

    public bool RightOuterWeaponPressed { get; private set; }
    public bool RightOuterWeaponReleased { get; private set; }
    public bool RightOuterWeaponHeld { get; private set; }

    public bool reloadPressed { get; private set; }
    public bool reloadReleased { get; private set; }
    public bool reloadHeld { get; private set; }

    public bool healPressed { get; private set; }
    public bool healReleased { get; private set; }
    public bool healHeld { get; private set; }

    public bool dodgePressed { get; private set; }
    public bool dodgeReleased { get; private set; }
    public bool dodgeHeld { get; private set; }

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

        LeftInnerWeaponPressed = PlayerInputActions.Gameplay.FireLeftInnerWeapon.WasPressedThisFrame();
        LeftInnerWeaponReleased = PlayerInputActions.Gameplay.FireLeftInnerWeapon.WasReleasedThisFrame();
        LeftInnerWeaponHeld = PlayerInputActions.Gameplay.FireLeftInnerWeapon.IsPressed();

        LeftOuterWeaponPressed = PlayerInputActions.Gameplay.FireLeftOuterWeapon.WasPressedThisFrame();
        LeftOuterWeaponReleased = PlayerInputActions.Gameplay.FireLeftOuterWeapon.WasReleasedThisFrame();
        LeftOuterWeaponHeld = PlayerInputActions.Gameplay.FireLeftOuterWeapon.IsPressed();

        RightInnerWeaponPressed = PlayerInputActions.Gameplay.FireRightInnerWeapon.WasPressedThisFrame();
        RightInnerWeaponReleased = PlayerInputActions.Gameplay.FireRightInnerWeapon.WasReleasedThisFrame();
        RightInnerWeaponHeld = PlayerInputActions.Gameplay.FireRightInnerWeapon.IsPressed();

        RightOuterWeaponPressed = PlayerInputActions.Gameplay.FireRightOuterWeapon.WasPressedThisFrame();
        RightOuterWeaponReleased = PlayerInputActions.Gameplay.FireRightOuterWeapon.WasReleasedThisFrame();
        RightOuterWeaponHeld = PlayerInputActions.Gameplay.FireRightOuterWeapon.IsPressed();

        reloadPressed = PlayerInputActions.Gameplay.Reload.WasPressedThisFrame();
        reloadReleased = PlayerInputActions.Gameplay.Reload.WasReleasedThisFrame();
        reloadHeld = PlayerInputActions.Gameplay.Reload.IsPressed();

        healPressed = PlayerInputActions.Gameplay.Heal.WasPressedThisFrame();
        healReleased = PlayerInputActions.Gameplay.Heal.WasReleasedThisFrame();
        healHeld = PlayerInputActions.Gameplay.Heal.IsPressed();

        dodgePressed = PlayerInputActions.Gameplay.Dodge.WasPressedThisFrame();
        dodgeReleased = PlayerInputActions.Gameplay.Dodge.WasReleasedThisFrame();
        dodgeHeld = PlayerInputActions.Gameplay.Dodge.IsPressed();
    }
}
