
using UnityEngine;

public class TurretShoot : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("STATS")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float projectileSpeed = 1f;
    [SerializeField] private int minDamage = 5;
    [SerializeField] private int maxDamage = 10;
    [SerializeField] private float criticalChance = 10f;
    [SerializeField] private float criticalMultiplier = 2f;
    private float fireTimer;
    public bool isShooting = false;


    private void Update()
    {
        isShooting = (InputController.Instance.MainWeaponHeld);
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            if (isShooting)
            {
                Fire();
                fireTimer = 0f;
            }
        }
    }

    private void Fire()
    {
        var bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        var projectile = bullet.GetComponent<Projectile>();
        projectile?.Initialize(projectileSpeed, minDamage, maxDamage, criticalChance, criticalMultiplier);
    }
}
