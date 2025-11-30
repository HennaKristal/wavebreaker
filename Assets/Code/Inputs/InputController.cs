using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private PlayerInputActions playerInputActions;
    private PlayerInputActions.GameplayActions gameplayInputs;

    public Vector2 Move { get; private set; }
    public Vector2 RotateGuns { get; private set; }
    public bool PausePressed { get; private set; }
    public bool ScreenshotPressed { get; private set; }
    public bool MainWeaponPressed { get; private set; }
    public bool MainWeaponHeld { get; private set; }
    public bool EnterPressed { get; private set; }
    public bool CancelPressed { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        playerInputActions = new PlayerInputActions();
        gameplayInputs = playerInputActions.Gameplay;
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        if (Instance == this && playerInputActions != null)
        {
            playerInputActions.Disable();
        }
    }

    private void Update()
    {
        Move = gameplayInputs.Move.ReadValue<Vector2>();
        RotateGuns = gameplayInputs.RotateGuns.ReadValue<Vector2>();
        PausePressed = gameplayInputs.Pause.WasPressedThisFrame();
        ScreenshotPressed = gameplayInputs.Screenshot.WasPressedThisFrame();
        MainWeaponPressed = gameplayInputs.FireMainWeapon.WasPressedThisFrame();
        MainWeaponHeld = gameplayInputs.FireMainWeapon.IsPressed();
        EnterPressed = gameplayInputs.Enter.WasPressedThisFrame();
        CancelPressed = gameplayInputs.Cancel.WasPressedThisFrame();
    }
}
