using UnityEngine;

public class TurretRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float rotationSpeedMultiplierWhileShooting = 0.4f;
    [SerializeField] private float mouseSwitchThreshold = 20f;
    [SerializeField] private float controllerSwitchThreshold = 0.25f;

    private Camera mainCamera;
    private TurretShoot turretShoot;

    private enum InputMethod { Mouse, Controller }
    private InputMethod currentInputMethod = InputMethod.Mouse;
    private Vector3 lastMousePos;
    private Vector3 currentMousePos;
    private Vector2 controllerInput;


    private void Start()
    {
        turretShoot = GetComponent<TurretShoot>();
        mainCamera = Camera.main;
        lastMousePos = Input.mousePosition;
    }


    private void Update()
    {
        controllerInput = InputController.Instance.RotateGuns;
        currentMousePos = Input.mousePosition;

        DetectInputMethod();
        HandleGunRotation();

        lastMousePos = currentMousePos;
    }


    private void HandleGunRotation()
    {
        float updatedRotationSpeed = rotationSpeed;
        if (turretShoot.isShooting)
        {
            updatedRotationSpeed = rotationSpeed * rotationSpeedMultiplierWhileShooting;
        }

        if (currentInputMethod == InputMethod.Controller)
        {
            if (controllerInput.magnitude > controllerSwitchThreshold)
            {
                float targetAngle = Mathf.Atan2(controllerInput.y, controllerInput.x) * Mathf.Rad2Deg;
                float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, updatedRotationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
        else
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(currentMousePos);
            mouseWorldPos.z = 0f;

            Vector3 direction = mouseWorldPos - transform.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, updatedRotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }


    private void DetectInputMethod()
    {
        if (controllerInput.magnitude > controllerSwitchThreshold)
        {
            currentInputMethod = InputMethod.Controller;
        }
        else if ((currentMousePos - lastMousePos).sqrMagnitude > mouseSwitchThreshold * mouseSwitchThreshold)
        {
            currentInputMethod = InputMethod.Mouse;
        }
    }
}
