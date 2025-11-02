using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 1f;
    [SerializeField] private float backwardSpeed = 0.5f;
    [SerializeField] private float turnSpeed = 100f;

    private InputController inputController;
    private Rigidbody2D rb;


    private void Start()
    {
        inputController = GameManager.Instance.GetComponent<InputController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 inputVector = inputController.Move;

        float turnAmount = -inputVector.x * turnSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + turnAmount);

        float speed = inputVector.y > 0 ? forwardSpeed : backwardSpeed;
        Vector2 forward = transform.up;
        rb.linearVelocity = forward * inputVector.y * speed;
    }
}
