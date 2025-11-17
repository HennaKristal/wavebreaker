using UnityEngine;

public class FlagshipHQ : MonoBehaviour
{
    [SerializeField] private float MovementSpeed;
    private Rigidbody2D rigidBody;


    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rigidBody.linearVelocity = transform.up * MovementSpeed;
    }
}
