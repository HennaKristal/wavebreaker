using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyHealthBase : MonoBehaviour
{
    [Header("STATS")]
    [SerializeField] protected int maxHealth = 50;
    [SerializeField] protected int collisionDamage = 100;
    protected int currentHealth = 50;

    [Header("DEATH")]
    [SerializeField] protected GameObject explosionPrefab;
    [SerializeField] protected float explosionDuration = 0f;
    [SerializeField] protected int explosionAmount = 1;
    [SerializeField] protected float explosionRadius = 0f;

    [Header("REFERENCES")]
    [SerializeField] protected GameObject damageNumberPrefab;
    protected Collider2D enemyCollider;
    protected bool isDead = false;

    [Header("RESOURCE DROPS")]
    [SerializeField] protected List<ResourceDrop> resourceDrops = new List<ResourceDrop>();
    [SerializeField] protected float dropRadius = 0.1f;

    protected SpriteRenderer sr;
    protected Material defaultMaterial;
    protected Material flashMaterial;


    protected virtual void Start()
    {
        enemyCollider = GetComponent<Collider2D>();

        sr = GetComponent<SpriteRenderer>();
        defaultMaterial = sr.material;
        flashMaterial = GameManager.Instance.GetEnemyDamageFlashMaterial();

        currentHealth = maxHealth;
    }


    public virtual void TakeDamage(int damage, bool isCritical)
    {
        if (isDead) { return; }

        currentHealth -= damage;

        if (damageNumberPrefab != null)
        {
            GameObject damageNumber = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity);
            damageNumber.GetComponent<DamageNumber>().Initialize((int)damage, isCritical);
        }

        UpdateHealthBar();

        sr.material = flashMaterial;
        StartCoroutine(RevertDamageFlash());

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            enemyCollider.enabled = false;

            GiveKillRewards();

            if (explosionPrefab != null && explosionAmount > 0)
            {
                StartCoroutine(SpawnExplosionsOverTime());
                Destroy(gameObject, explosionDuration + 0.1f);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }


    protected virtual IEnumerator RevertDamageFlash()
    {
        yield return new WaitForSeconds(0.1f);
        sr.material = defaultMaterial;
    }


    protected virtual void GiveKillRewards()
    {
        foreach (ResourceDrop drop in resourceDrops)
        {
            if (drop.prefab == null || drop.amount <= 0) continue;

            for (int i = 0; i < drop.amount; i++)
            {
                // Random offset within a circle
                Vector2 offset = Random.insideUnitCircle * dropRadius;
                Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0);

                Instantiate(drop.prefab, spawnPos, Quaternion.identity);
            }
        }
    }



    protected virtual IEnumerator SpawnExplosionsOverTime()
    {
        float interval = (explosionAmount > 1) ? explosionDuration / (explosionAmount - 1) : 0f;

        for (int i = 0; i < explosionAmount; i++)
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * explosionRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            Instantiate(explosionPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(interval);
        }
    }


    public int GetCollisionDamage()
    {
        return collisionDamage;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }


    protected abstract void UpdateHealthBar();
}
