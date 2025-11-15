using UnityEngine;

public class InputController : MonoBehaviour
{
    private static InputController _instance;
    public static InputController Instance => _instance;

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


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        playerInputActions = new PlayerInputActions();
        gameplayInputs = playerInputActions.Gameplay;
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
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
