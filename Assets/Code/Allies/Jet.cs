using UnityEngine;
using System.Collections;

public class Jet : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float accelerationTime = 2f;
    [SerializeField] private float turnSpeed = 180f;
    [SerializeField] private float flyPastDuration = 2f;

    [Header("Combat")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int burstCount = 12;
    [SerializeField] private float burstDelay = 0.25f;
    [SerializeField] private float projectileSpeed = 1.2f;
    [SerializeField] private int minDamage = 3;
    [SerializeField] private int maxDamage = 5;
    [SerializeField] private float criticalChance = 5;
    [SerializeField] private float criticalMultiplier = 1.5f;

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 30f;

    private float currentSpeed = 0f;
    private Transform currentTarget;


    private void Start()
    {
        StartCoroutine(JetRoutine());
        Destroy(gameObject, lifeTime);
    }

    private IEnumerator JetRoutine()
    {
        // PHASE 1: Acceleration
        float t = 0;
        while (t < accelerationTime)
        {
            t += Time.deltaTime;
            currentSpeed = Mathf.Lerp(0, maxSpeed, t / accelerationTime);
            transform.position += transform.up * currentSpeed * Time.deltaTime;
            yield return null;
        }

        // Main behavior loop
        while (true)
        {
            AcquireTarget();

            if (currentTarget)
            {
                // Rotate toward target
                yield return StartCoroutine(RotateTowards(currentTarget));

                // Fire burst
                yield return StartCoroutine(FireBurst());

                // Fly past target for a moment
                yield return new WaitForSeconds(flyPastDuration);
            }
            else
            {
                // No target: just fly straight
                transform.position += transform.up * maxSpeed * Time.deltaTime;
                yield return null;
            }
        }
    }

    private void Update()
    {
        // Continuous forward movement
        transform.position += transform.up * currentSpeed * Time.deltaTime;
    }

    private void AcquireTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            currentTarget = null;
            return;
        }

        currentTarget = enemies[Random.Range(0, enemies.Length)].transform;
    }

    private IEnumerator RotateTowards(Transform target)
    {
        while (target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            Quaternion goalRotation = Quaternion.Euler(0, 0, angle);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                goalRotation,
                turnSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(transform.rotation, goalRotation) < 2f)
            {
                break;
            }

            yield return null;
        }
    }

    private IEnumerator FireBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            var projectile = bullet.GetComponent<Projectile>();
            projectile?.Initialize(projectileSpeed, minDamage, maxDamage, criticalChance, criticalMultiplier);
            yield return new WaitForSeconds(burstDelay);
        }
    }
}
