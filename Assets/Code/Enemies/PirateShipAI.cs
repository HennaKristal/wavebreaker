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
    private Rigidbody2D rb;

    private enum State { Approaching, Strafing }
    private State currentState = State.Approaching;

    private float fireTimer = 0f;
    private float sideFireTimer = 0f;
    private bool sideGunEnabled = false;

    private Transform currentTarget;
    [SerializeField] private float targetRefreshRate = 1f;
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

        void CheckGroup(GameObject[] arr)
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

        CheckGroup(players);
        CheckGroup(allies);

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

        UpdateState();

        switch (currentState)
        {
            case State.Approaching:
                MoveTowardTarget();
                break;

            case State.Strafing:
                StrafeAroundTarget();

                if (mainGunEnabled)
                {
                    RotateGunTowardTarget();
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
        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

        if (currentState == State.Approaching && distanceToTarget <= idealRange)
        {
            currentState = State.Strafing;
        }
        else if (currentState == State.Strafing && distanceToTarget > idealRange + rangeBuffer)
        {
            currentState = State.Approaching;
        }
    }

    private void MoveTowardTarget()
    {
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        RotateTowards(direction);
        rb.linearVelocity = transform.up * approachSpeed;
    }

    private void RotateTowards(Vector2 targetDirection)
    {
        float angle = Vector2.SignedAngle(transform.up, targetDirection);
        float step = Mathf.Clamp(angle, -turnSpeed * Time.fixedDeltaTime, turnSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation + step);
    }

    private void StrafeAroundTarget()
    {
        Vector2 toTarget = (currentTarget.position - transform.position).normalized;
        Vector2 strafeDir = Vector2.Perpendicular(toTarget);
        RotateTowards(strafeDir);
        rb.linearVelocity = transform.up * strafeSpeed;
    }

    private void RotateGunTowardTarget()
    {
        if (!gunTransform || !currentTarget) { return; }

        Vector3 direction = currentTarget.position - gunTransform.position;

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
