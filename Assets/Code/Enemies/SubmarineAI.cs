using System.Collections;
using UnityEngine;

public class BossSubmarineAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private float minSubmergedDuration = 5f;
    [SerializeField] private float maxSubmergedDuration = 8f;

    [Header("Shooting Settings")]
    [SerializeField] private Transform frontGunTransform;
    [SerializeField] private Transform frontGunFirePoint;
    [SerializeField] private Transform rearGunTransform;
    [SerializeField] private Transform rearGunFirePoint;
    [SerializeField] private float gunRotationSpeed = 180f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private int burstAmount = 10;
    [SerializeField] private float projectileSpeed = 2f;
    [SerializeField] private int minDamage = 5;
    [SerializeField] private int maxDamage = 10;

    [Header("Missile Settings")]
    [SerializeField] private GameObject NukePrefabWarning;
    [SerializeField] private GameObject MissileWarningPrefab;
    [SerializeField] private float missileRadius = 2f;
    [SerializeField] private float missileDelay = 0.2f;

    private EnemyHealthBase enemyHealthBase;
    private Animator animator;
    private Transform playerTransform;
    private bool isFiringGuns = false;
    private bool nukeAttackUsed = false;
    private float sweepAngle = 30f;
    private float sweepDuration = 0f;
    private float sweepTimer = 0f;
    private float sweepBaseAngle;


    private void Start()
    {
        playerTransform = GameManager.Instance.GetPlayerTransform();
        enemyHealthBase = GetComponent<EnemyHealthBase>();
        animator = GetComponent<Animator>();
        Ascend();
    }


    private void Update()
    {
        RotateGunsTowardPlayer();

        if (enemyHealthBase.isDead)
        {
            CancelInvoke(nameof(Attack));
            CancelInvoke(nameof(Submerge));
            CancelInvoke(nameof(Ascend));
        }
    }


    private void Ascend()
    {
        TeleportToSurfacePosition();
        animator.SetTrigger("Emerge");
        Invoke(nameof(Attack), 4f);
    }


    private void Attack()
    {
        if (!nukeAttackUsed && enemyHealthBase.GetCurrentHealth() * 4 < enemyHealthBase.GetMaxHealth())
        {
            PerformNukeAttack();
        }
        else
        {
            int firstAttack = Random.Range(0, 2);
            int secondAttack;

            switch (firstAttack)
            {
                case 0: PerformMissileAttack(); break;
                case 1: PerformTurretAttacks(); break;
            }

            if (enemyHealthBase.GetCurrentHealth() * 2 < enemyHealthBase.GetMaxHealth())
            {
                do { secondAttack = Random.Range(0, 2); }
                while (secondAttack == firstAttack);

                switch (secondAttack)
                {
                    case 0: PerformMissileAttack(); break;
                    case 1: PerformTurretAttacks(); break;
                }
            }
        }

        Invoke(nameof(Submerge), 6f);
    }


    private void Submerge()
    {
        animator.SetTrigger("Submerge");
        Invoke(nameof(Ascend), Random.Range(minSubmergedDuration, maxSubmergedDuration));
    }


    private void TeleportToSurfacePosition()
    {
        Vector2 offset = Random.insideUnitCircle * spawnRadius;
        Vector3 newPos = playerTransform.position + new Vector3(offset.x, offset.y, 0f);
        transform.position = newPos;
    }


    private void PerformMissileAttack()
    {
        animator.SetTrigger("MissileAttack");
        StartCoroutine(PerformMissileAttackCoroutine());
    }


    private IEnumerator PerformMissileAttackCoroutine()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 8; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * missileRadius;
            Vector3 targetPos = playerTransform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
            Instantiate(MissileWarningPrefab, targetPos, Quaternion.identity);
            yield return new WaitForSeconds(missileDelay);
        }
    }



    private void PerformNukeAttack()
    {
        nukeAttackUsed = true;
        animator.SetTrigger("NukeAttack");
        Instantiate(NukePrefabWarning, playerTransform.position, Quaternion.identity);
    }


    private void PerformTurretAttacks()
    {
        StartCoroutine(FireTurrets());
    }


    private IEnumerator FireTurrets()
    {
        isFiringGuns = true;

        // Store the base angle once, before we start sweeping
        Vector3 toPlayer = playerTransform.position - frontGunTransform.position;
        sweepBaseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        sweepTimer = 0f;
        sweepDuration = burstAmount * fireRate;

        for (int i = 0; i < burstAmount; i++)
        {
            // Front gun
            GameObject frontBullet = Instantiate(projectilePrefab, frontGunFirePoint.position, frontGunFirePoint.rotation);
            var frontProjectile = frontBullet.GetComponent<EnemyProjectile>();
            frontProjectile?.Initialize(projectileSpeed, minDamage, maxDamage);

            // Rear gun
            GameObject rearBullet = Instantiate(projectilePrefab, rearGunFirePoint.position, rearGunFirePoint.rotation);
            var rearProjectile = rearBullet.GetComponent<EnemyProjectile>();
            rearProjectile?.Initialize(projectileSpeed, minDamage, maxDamage);

            yield return new WaitForSeconds(fireRate);
        }

        isFiringGuns = false;
    }


    private void RotateGunsTowardPlayer()
    {
        if (playerTransform == null && !enemyHealthBase.isDead) return;

        // Rear gun always tracks player
        Vector3 direction = playerTransform.position - rearGunTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float current = rearGunTransform.eulerAngles.z;
        float smoothAngle = Mathf.MoveTowardsAngle(current, angle, gunRotationSpeed * Time.deltaTime);
        rearGunTransform.rotation = Quaternion.Euler(0f, 0f, smoothAngle);

        if (isFiringGuns)
        {
            // Front gun sweep
            sweepTimer += Time.deltaTime;
            float t = Mathf.Clamp01(sweepTimer / sweepDuration);
            float offset = Mathf.Lerp(-sweepAngle, sweepAngle, t);
            frontGunTransform.rotation = Quaternion.Euler(0f, 0f, sweepBaseAngle + offset);
        }
        else
        {
            // Front gun track player with offset
            direction = playerTransform.position - frontGunTransform.position;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - sweepAngle;
            current = frontGunTransform.eulerAngles.z;
            smoothAngle = Mathf.MoveTowardsAngle(current, angle, gunRotationSpeed * Time.deltaTime);
            frontGunTransform.rotation = Quaternion.Euler(0f, 0f, smoothAngle);
        }
    }
}
