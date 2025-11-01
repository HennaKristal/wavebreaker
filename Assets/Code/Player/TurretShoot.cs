using UnityEngine;

public class TurretShoot : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;

    [Header("Projectile Stats")]
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private int minDamage = 5;
    [SerializeField] private int maxDamage = 10;
    [SerializeField] private float criticalChance = 10f; // percent
    [SerializeField] private float criticalMultiplier = 2f;

    private float fireTimer;

    private void Update()
    {
        fireTimer += Time.deltaTime;

        if (Input.GetMouseButton(0) && fireTimer >= fireRate)
        {
            Fire();
            fireTimer = 0f;
        }
    }

    private void Fire()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile proj = projectile.GetComponent<Projectile>();
        proj?.Initialize(projectileSpeed, minDamage, maxDamage, criticalChance, criticalMultiplier);
    }
}
