using UnityEngine;

public class PirateShipAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float approachSpeed = 1.2f;
    [SerializeField] private float strafeSpeed = 1.0f;
    [SerializeField] private float turnSpeed = 50f;
    [SerializeField] private float idealRange = 5f;
    [SerializeField] private float rangeBuffer = 1f;

    [Header("Main Shooting Settings")]
    [SerializeField] private bool mainGunEnabled = false;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private float gunRotationSpeed = 180f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate = 1.0f;
    [SerializeField] private float projectileSpeed = 2f;
    [SerializeField] private int minDamage = 5;
    [SerializeField] private int maxDamage = 10;

    [Header("Side Shooting Settings")]
    [SerializeField] private Transform sideFirePointRight;
    [SerializeField] private Transform sideFirePointLeft;
    [SerializeField] private GameObject sideProjectilePrefab;
    [SerializeField] private float sideFireRate = 1.0f;
    [SerializeField] private float sideProjectileSpeed = 2f;
    [SerializeField] private int sideMinDamage = 5;
    [SerializeField] private int sideMaxDamage = 10;
    [SerializeField] private int sideShotCount = 5;
    [SerializeField] private float sideShotSpreadAngle = 45f;


    private Transform sideFirePoint;
    private Transform playerTransform;
    private Rigidbody2D rb;

    private enum State { Approaching, Strafing }
    private State currentState = State.Approaching;

    private float fireTimer = 0f;
    private float sideFireTimer = 0f;
    private bool sideGunEnabled = false;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameManager.Instance.GetPlayerTransform();
    }


    private void FixedUpdate()
    {
        if (playerTransform == null) { return; }

        UpdateState();

        switch (currentState)
        {
            case State.Approaching:
                MoveTowardPlayer();
                break;

            case State.Strafing:
                StrafeAroundPlayer();

                if (mainGunEnabled)
                {
                    RotateGunTowardPlayer();
                    HandleShooting();
                }

                break;
        }

        if (sideGunEnabled)
        {
            HandleSideShooting();
        }
    }


    private void UpdateState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (currentState == State.Approaching && distanceToPlayer <= idealRange)
        {
            currentState = State.Strafing;
        }
        else if (currentState == State.Strafing && distanceToPlayer > idealRange + rangeBuffer)
        {
            currentState = State.Approaching;
        }
    }


    private void MoveTowardPlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        RotateTowards(direction);
        rb.linearVelocity = transform.up * approachSpeed;
    }


    private void RotateTowards(Vector2 targetDirection)
    {
        float angle = Vector2.SignedAngle(transform.up, targetDirection);
        float step = Mathf.Clamp(angle, -turnSpeed * Time.fixedDeltaTime, turnSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation + step);
    }


    private void StrafeAroundPlayer()
    {
        Vector2 toPlayer = (playerTransform.position - transform.position).normalized;
        Vector2 strafeDir = Vector2.Perpendicular(toPlayer);
        RotateTowards(strafeDir);
        rb.linearVelocity = transform.up * strafeSpeed;
    }


    private void RotateGunTowardPlayer()
    {
        if (!gunTransform || !playerTransform) { return; }

        Vector3 direction = playerTransform.position - gunTransform.position;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float currentAngle = gunTransform.eulerAngles.z;
        float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, gunRotationSpeed * Time.deltaTime);

        gunTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }


    private void HandleShooting()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            Fire(projectilePrefab, firePoint.position, firePoint.rotation, projectileSpeed, minDamage, maxDamage);
            fireTimer = 0f;
        }
    }


    private void HandleSideShooting()
    {
        sideFireTimer += Time.deltaTime;

        if (sideFireTimer >= sideFireRate)
        {
            float startAngle = -sideShotSpreadAngle / 2f;
            float angleStep = sideShotSpreadAngle / (sideShotCount - 1);

            for (int i = 0; i < sideShotCount; i++)
            {
                float angle = startAngle + angleStep * i;
                Quaternion rotation = sideFirePoint.rotation * Quaternion.Euler(0f, 0f, angle);
                Fire(sideProjectilePrefab, sideFirePoint.position, rotation, sideProjectileSpeed, sideMinDamage, sideMaxDamage);
            }

            sideFireTimer = 0f;
        }
    }


    private void Fire(GameObject prefab, Vector3 position, Quaternion rotation, float speed, int minDmg, int maxDmg)
    {
        if (!prefab) return;

        GameObject bullet = Instantiate(prefab, position, rotation);

        if (bullet.TryGetComponent(out EnemyProjectile enemyProjectile))
        {
            enemyProjectile.Initialize(speed, minDmg, maxDmg);
        }
    }



    public void EnableSideGuns(bool useRightSide)
    {
        sideGunEnabled = true;
        sideFirePoint = useRightSide ? sideFirePointRight : sideFirePointLeft;
    }


    public void DisableSideGuns()
    {
        sideGunEnabled = false;
    }
}
