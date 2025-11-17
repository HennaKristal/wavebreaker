using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 1f;
    [SerializeField] private float backwardSpeed = 0.5f;
    [SerializeField] private float turnSpeed = 100f;
    private Rigidbody2D rb;

    [Header("Idle Boat Animation")]
    [SerializeField] private float idleDirection = 1f;
    [SerializeField] private float idleSpeed = 1f;
    [SerializeField] private float idleTurnSpeed = 40f;
    [SerializeField] private float flipCenter = 0f;
    [SerializeField] private float flipThreshold = 15f;
    private bool canFlip = true;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.gameStarted)
        {
            IdleBoatAnimation();
            return;
        }

        Move();
    }

    private void Move()
    {
        Vector2 inputVector = InputController.Instance.Move;
        float turnAmount = -inputVector.x * turnSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + turnAmount);

        float finalSpeed = inputVector.y > 0 ? forwardSpeed : backwardSpeed;
        Vector2 forward = transform.up;
        rb.linearVelocity = forward * inputVector.y * finalSpeed;
    }

    private void IdleBoatAnimation()
    {
        Vector2 fakeInput = new Vector2(idleDirection, 1f);

        float turnAmount = -fakeInput.x * idleTurnSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + turnAmount);

        Vector2 forward = transform.up;
        rb.linearVelocity = forward * fakeInput.y * idleSpeed;

        float z = rb.rotation % 360f;
        if (z < 0) z += 360f;

        bool inFlipZone = Mathf.Abs(Mathf.DeltaAngle(z, flipCenter)) < flipThreshold;

        if (canFlip && inFlipZone)
        {
            idleDirection *= -1f;
            canFlip = false;
        }

        if (!inFlipZone)
        {
            canFlip = true;
        }
    }
}
