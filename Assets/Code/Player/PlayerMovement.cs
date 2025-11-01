using UnityEngine;


public class PlayerMovement: MonoBehaviour
{
    [SerializeField] private float horizontalSpeed = 5;
    [SerializeField] private float verticalSpeed = 5;

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

        if (inputVector.magnitude > 1f)
        {
            inputVector.Normalize();
        }

        float xSpeed = inputVector.x * horizontalSpeed;
        float ySpeed = inputVector.y * verticalSpeed;

        rb.linearVelocity = new Vector2(xSpeed, ySpeed);
    }
}
