using UnityEngine;

public class AllyTurret : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireDelay = 1.0f;
    [SerializeField] private float projectileSpeed = 2f;
    [SerializeField] private int minDamage = 5;
    [SerializeField] private int maxDamage = 10;
    [SerializeField] private float gunRotationSpeed = 180f;
    [SerializeField] private Vector2 rotationLimits = new Vector2(0, 180);
    [SerializeField] private float shootingRange = 5f;
    [SerializeField] private float targetRefreshRate = 5f;
    private Transform currentTarget;
    private float targetTimer;
    private float fireTimer;
    private float distanceToClosestTarget;
    private bool targetWithinRotationLimits;


    private void Start()
    {
        SelectNewTarget();
    }

    private void SelectNewTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        Transform closest = null;
        distanceToClosestTarget = Mathf.Infinity;

        void CheckTargets(GameObject[] arr)
        {
            foreach (var obj in arr)
            {
                float distanceToTarget = Vector2.Distance(transform.position, obj.transform.position);
                if (distanceToTarget < distanceToClosestTarget)
                {
                    distanceToClosestTarget = distanceToTarget;
                    closest = obj.transform;
                }
            }
        }

        CheckTargets(enemies);
        currentTarget = closest;
    }

    private void Update()
    {
        if (currentTarget == null)
        {
            SelectNewTarget();
            return;
        }

        targetTimer += Time.deltaTime;
        if (targetTimer >= targetRefreshRate)
        {
            SelectNewTarget();
            targetTimer = 0f;
        }

        distanceToClosestTarget = Vector2.Distance(transform.position, currentTarget.position);

        if (distanceToClosestTarget < shootingRange)
        {
            RotateGunTowardTarget();
            if (targetWithinRotationLimits)
            {
                HandleShooting();
            }
        }
    }

    private void RotateGunTowardTarget()
    {
        if (currentTarget == null) { return; }

        Vector3 direction = currentTarget.position - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Determine if the target angle is inside allowed rotation sector
        targetWithinRotationLimits = IsAngleWithinLimits(targetAngle, rotationLimits.x, rotationLimits.y);

        float clampedTargetAngle = ClampAngleToLimits(targetAngle, rotationLimits.x, rotationLimits.y);
        float currentAngle = transform.eulerAngles.z;
        float angle = Mathf.MoveTowardsAngle(currentAngle, clampedTargetAngle, gunRotationSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private static float NormalizeAngle360(float angle)
    {
        angle %= 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }

    private static float ClampAngleToLimits(float angle, float minLimit, float maxLimit)
    {
        float a = NormalizeAngle360(angle);
        float min = NormalizeAngle360(minLimit);
        float max = NormalizeAngle360(maxLimit);

        if (min <= max)
        {
            return Mathf.Clamp(a, min, max);
        }
        else
        {
            bool inside = (a >= min) || (a <= max);
            if (inside) return a;

            float dToMin = Mathf.Abs(Mathf.DeltaAngle(a, min));
            float dToMax = Mathf.Abs(Mathf.DeltaAngle(a, max));
            return dToMin < dToMax ? min : max;
        }
    }

    private static bool IsAngleWithinLimits(float angle, float minLimit, float maxLimit)
    {
        float a = NormalizeAngle360(angle);
        float min = NormalizeAngle360(minLimit);
        float max = NormalizeAngle360(maxLimit);

        if (min <= max)
        {
            return a >= min && a <= max;
        }
        else
        {
            return (a >= min) || (a <= max);
        }
    }

    private void HandleShooting()
    {
        if (projectilePrefab == null || firePoint == null || currentTarget == null)
        {
            return;
        }

        // If the target is outside rotation limits, don't shoot
        if (!targetWithinRotationLimits) return;

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireDelay)
        {
            Fire();
            fireTimer = 0f;
        }
    }

    private void Fire()
    {
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        var projectile = bullet.GetComponent<Projectile>();
        projectile?.Initialize(projectileSpeed, minDamage, maxDamage, 0, 1);
    }
}
