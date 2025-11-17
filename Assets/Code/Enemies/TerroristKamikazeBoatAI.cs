using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TerroristKamikazeBoatAI : MonoBehaviour
{
    private Transform playerTransform;
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    [SerializeField] private float approachSpeed = 0.8f;
    [SerializeField] private float rammingSpeed = 2.0f;
    [SerializeField] private float speedLerpSpeed = 2.5f;
    [SerializeField] private float turnSpeed = 40f;
    [SerializeField] private float rammingAngleThreshold = 30f;
    private float currentSpeed = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameManager.Instance.GetPlayerTransform();
        currentSpeed = approachSpeed;
    }

    private void FixedUpdate()
    {
        if (playerTransform == null) return;

        Vector2 vectorToPlayer = (playerTransform.position - transform.position).normalized;
        float angleToPlayer = Vector2.SignedAngle(transform.up, vectorToPlayer);

        float rotationStep = Mathf.Clamp(angleToPlayer, -turnSpeed * Time.fixedDeltaTime, turnSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation + rotationStep);

        float targetSpeed = Mathf.Abs(angleToPlayer) <= rammingAngleThreshold ? rammingSpeed : approachSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, speedLerpSpeed * Time.fixedDeltaTime);
        rb.linearVelocity = transform.up * currentSpeed;
    }
}
