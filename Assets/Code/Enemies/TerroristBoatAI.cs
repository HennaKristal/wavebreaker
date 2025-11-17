using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TerroristBoatAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float approachSpeed = 1.2f;
    [SerializeField] private float strafeSpeed = 1.0f;
    [SerializeField] private float turnSpeed = 50f;
    [SerializeField] private float idealRange = 5f;
    [SerializeField] private float rangeBuffer = 1f;

    [Header("Shooting Settings")]
    [SerializeField] private Transform gunTransform;
    [SerializeField] private float gunRotationSpeed = 180f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate = 1.0f;
    [SerializeField] private float projectileSpeed = 2f;
    [SerializeField] private int minDamage = 5;
    [SerializeField] private int maxDamage = 10;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private enum State { Approaching, Strafing }
    private State currentState;
    private float fireTimer;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameManager.Instance.GetPlayerTransform();
    }

    private void FixedUpdate()
    {
        if (playerTransform == null) { return; }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (currentState == State.Approaching && distanceToPlayer <= idealRange)
        {
            currentState = State.Strafing;
        }
        else if (currentState == State.Strafing && distanceToPlayer > idealRange + rangeBuffer)
        {
            currentState = State.Approaching;
        }

        switch (currentState)
        {
            case State.Approaching:
                MoveTowardPlayer();
                break;
            case State.Strafing:
                StrafeAroundPlayer();
                RotateGunTowardPlayer();
                HandleShooting();
                break;
        }
    }

    private void MoveTowardPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        float angleToPlayer = Vector2.SignedAngle(transform.up, direction);
        float rotationStep = Mathf.Clamp(angleToPlayer, -turnSpeed * Time.fixedDeltaTime, turnSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(rb.rotation + rotationStep);
        rb.linearVelocity = transform.up * approachSpeed;
    }

    private void StrafeAroundPlayer()
    {
        Vector2 vectorToPlayer = (playerTransform.position - transform.position).normalized;
        Vector2 strafeDirection = Vector2.Perpendicular(vectorToPlayer);

        float angleToStrafe = Vector2.SignedAngle(transform.up, strafeDirection);
        float rotationStep = Mathf.Clamp(angleToStrafe, -turnSpeed * Time.fixedDeltaTime, turnSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(rb.rotation + rotationStep);
        rb.linearVelocity = transform.up * strafeSpeed;
    }

    private void RotateGunTowardPlayer()
    {
        if (gunTransform == null || playerTransform == null) { return; }

        Vector3 direction = playerTransform.position - gunTransform.position;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float currentAngle = gunTransform.eulerAngles.z;
        float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, gunRotationSpeed * Time.deltaTime);

        gunTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void HandleShooting()
    {
        if (projectilePrefab == null || firePoint == null || playerTransform == null)
        {
            return;
        }

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            Fire();
            fireTimer = 0f;
        }
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        EnemyProjectile enemyProjectile = bullet.GetComponent<EnemyProjectile>();
        enemyProjectile?.Initialize(projectileSpeed, minDamage, maxDamage);
    }
}
