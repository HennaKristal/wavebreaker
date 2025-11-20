using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    [SerializeField] private float endFlyAwayDuration = 5f;
    [SerializeField] private float carrierTargetRange = 20f;

    private Transform carrier;
    private bool flyingAway = false;
    private float currentSpeed = 0f;
    private Transform currentTarget;


    private void Start()
    {
        StartCoroutine(JetRoutine());
        StartCoroutine(LifetimeRoutine());
    }

    public void SetCarrier(Transform carrierTransform)
    {
        carrier = carrierTransform;
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
            if (flyingAway)
            {
                yield return null;
                continue;
            }

            AcquireTarget();

            if (currentTarget)
            {
                yield return StartCoroutine(RotateTowards(currentTarget));

                // Only fire if target is NOT the carrier
                if (currentTarget != carrier)
                {
                    yield return StartCoroutine(FireBurst());
                }

                yield return new WaitForSeconds(flyPastDuration);
            }
            else
            {
                transform.position += transform.up * maxSpeed * Time.deltaTime;
                yield return null;
            }
        }

    }

    private IEnumerator LifetimeRoutine()
    {
        yield return new WaitForSeconds(lifeTime);

        flyingAway = true;
        currentTarget = null;
        currentSpeed = maxSpeed;

        Quaternion lockedRotation = transform.rotation;

        float t = 0f;
        while (t < endFlyAwayDuration)
        {
            transform.rotation = lockedRotation;
            transform.position += transform.up * currentSpeed * Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }


    private void Update()
    {
        if (!flyingAway)
        {
            transform.position += transform.up * currentSpeed * Time.deltaTime;
        }
    }


    private void AcquireTarget()
    {
        if (flyingAway)
        {
            currentTarget = null;
            return;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length > 0)
        {
            // Filter enemies to those within carrierTargetRange of the carrier (if carrier exists)
            List<GameObject> valid = new List<GameObject>();
            for (int i = 0; i < enemies.Length; i++)
            {
                var e = enemies[i];
                if (e == null) continue;

                if (carrier == null)
                {
                    // If carrier not assigned, consider all enemies valid
                    valid.Add(e);
                }
                else
                {
                    float dist = Vector2.Distance(e.transform.position, carrier.position);
                    if (dist <= carrierTargetRange)
                    {
                        valid.Add(e);
                    }
                }
            }

            if (valid.Count > 0)
            {
                currentTarget = valid[Random.Range(0, valid.Count)].transform;
                return;
            }
        }

        // No enemies in range: orbit/target the carrier
        currentTarget = carrier;
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
