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

    private Rigidbody2D rb;
    private enum State { Approaching, Strafing }
    private State currentState;
    private float fireTimer;

    [SerializeField] private Transform currentTarget;
    [SerializeField] private float targetRefreshRate = 5f;
    private float targetTimer;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SelectNewTarget();
        idealRange = Random.Range(idealRange - 1f, idealRange + 1f);
    }

    private void SelectNewTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");

        Transform closest = null;
        float closestDist = Mathf.Infinity;

        void CheckTargets(GameObject[] arr)
        {
            foreach (var obj in arr)
            {
                float d = Vector2.Distance(transform.position, obj.transform.position);
                if (d < closestDist)
                {
                    closestDist = d;
                    closest = obj.transform;
                }
            }
        }

        CheckTargets(players);
        CheckTargets(allies);

        currentTarget = closest;
    }


    private void FixedUpdate()
    {
        if (currentTarget == null)
        {
            SelectNewTarget();
            return;
        }

        targetTimer += Time.fixedDeltaTime;
        if (targetTimer >= targetRefreshRate)
        {
            SelectNewTarget();
            targetTimer = 0f;
        }

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

        if (currentState == State.Approaching && distanceToTarget <= idealRange)
        {
            currentState = State.Strafing;
        }
        else if (currentState == State.Strafing && distanceToTarget > idealRange + rangeBuffer)
        {
            currentState = State.Approaching;
        }

        switch (currentState)
        {
            case State.Approaching:
                MoveTowardTarget();
                break;
            case State.Strafing:
                StrafeAroundTarget();
                RotateGunTowardTarget();
                HandleShooting();
                break;
        }
    }

    private void MoveTowardTarget()
    {
        Vector2 direction = (currentTarget.position - transform.position).normalized;

        float angleToTarget = Vector2.SignedAngle(transform.up, direction);
        float rotationStep = Mathf.Clamp(angleToTarget, -turnSpeed * Time.fixedDeltaTime, turnSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(rb.rotation + rotationStep);
        rb.linearVelocity = transform.up * approachSpeed;
    }

    private void StrafeAroundTarget()
    {
        Vector2 vectorToTarget = (currentTarget.position - transform.position).normalized;
        Vector2 strafeDirection = Vector2.Perpendicular(vectorToTarget);

        float angleToStrafe = Vector2.SignedAngle(transform.up, strafeDirection);
        float rotationStep = Mathf.Clamp(angleToStrafe, -turnSpeed * Time.fixedDeltaTime, turnSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(rb.rotation + rotationStep);
        rb.linearVelocity = transform.up * strafeSpeed;
    }

    private void RotateGunTowardTarget()
    {
        if (gunTransform == null || currentTarget == null) { return; }

        Vector3 direction = currentTarget.position - gunTransform.position;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float currentAngle = gunTransform.eulerAngles.z;
        float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, gunRotationSpeed * Time.deltaTime);

        gunTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void HandleShooting()
    {
        if (projectilePrefab == null || firePoint == null || currentTarget == null)
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
