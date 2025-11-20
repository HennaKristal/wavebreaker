using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 1f;
    [SerializeField] private float backwardSpeed = 0.5f;
    [SerializeField] private float turnSpeed = 100f;
    private Rigidbody2D rigidBody;


    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 inputVector = InputController.Instance.Move;
        float turnAmount = -inputVector.x * turnSpeed * Time.fixedDeltaTime;
        rigidBody.MoveRotation(rigidBody.rotation + turnAmount);

        float finalSpeed = inputVector.y > 0 ? forwardSpeed : backwardSpeed;
        Vector2 forward = transform.up;
        rigidBody.linearVelocity = forward * inputVector.y * finalSpeed;
    }
}
