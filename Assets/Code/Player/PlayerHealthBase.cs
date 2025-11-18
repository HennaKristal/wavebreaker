using System.Collections;
using UnityEngine;

public abstract class PlayerHealthBase : MonoBehaviour
{
    [Header("REFERENCES")]
    protected Rigidbody2D rigidBody;
    [SerializeField] protected GameObject damageNumberPrefab;

    [Header("Stats")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int collisionDamage = 100;
    protected bool isDead = false;

    [Header("Damage Handling")]
    protected SpriteRenderer spriteRenderer;
    protected Material defaultMaterial;
    protected Material flashMaterial;
    protected float collisionDamageImmunityDuration = 1f;
    protected bool collisionDamageImmunity = false;

    [Header("DEATH")]
    [SerializeField] protected GameObject explosionPrefab;
    [SerializeField] protected float explosionDuration = 0f;
    [SerializeField] protected int explosionAmount = 1;
    [SerializeField] protected float explosionRadius = 0f;


    protected virtual void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;

        if (this.tag == "Ally")
        {
            flashMaterial = GameManager.Instance.GetAllyDamageFlashMaterial();
        }
        else
        {
            flashMaterial = GameManager.Instance.GetPlayerDamageFlashMaterial();
        }

        currentHealth = maxHealth;
        UpdateHealthUI();
    }


    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        PlayTakeDamageSound();

        int healthBeforeDamage = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (damageNumberPrefab != null)
        {
            GameObject damageNumber = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity);
            damageNumber.GetComponent<DamageNumber>().Initialize((int)damage, false);
        }

        if (currentHealth <= 0)
        {
            isDead = true;
            rigidBody.simulated = false;
            StartCoroutine(SpawnExplosionsOverTime());
        }

        UpdateHealthUI();

        spriteRenderer.material = flashMaterial;
        StartCoroutine(RevertDamageFlash());
    }


    protected virtual IEnumerator RevertDamageFlash()
    {
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material = defaultMaterial;
    }


    protected virtual IEnumerator SpawnExplosionsOverTime()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            Instantiate(explosionPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(0.25f);
        }
    }

    protected virtual void HandleEnemyCollision(GameObject enemyObject)
    {
        if (collisionDamageImmunity || isDead) { return; }

        collisionDamageImmunity = true;
        Invoke(nameof(CancelCollisionImmunity), collisionDamageImmunityDuration);

        var enemyHealth = enemyObject.GetComponent<EnemyHealthBase>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(collisionDamage, false);
            TakeDamage(enemyHealth.GetCollisionDamage());
        }
    }

    protected virtual void HandleAllyCollision(GameObject enemyObject)
    {
        if (collisionDamageImmunity || isDead) { return; }

        collisionDamageImmunity = true;
        Invoke(nameof(CancelCollisionImmunity), collisionDamageImmunityDuration);

        var allyHealth = enemyObject.GetComponent<PlayerHealthBase>();

        if (allyHealth != null)
        {
            allyHealth.TakeDamage(collisionDamage);
            TakeDamage(allyHealth.GetCollisionDamage());
        }
    }

    protected virtual void CancelCollisionImmunity()
    {
        collisionDamageImmunity = false;
    }


    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            CancelInvoke(nameof(CancelCollisionImmunity));
            collisionDamageImmunity = false;
            HandleEnemyCollision(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Ally"))
        {
            CancelInvoke(nameof(CancelCollisionImmunity));
            collisionDamageImmunity = false;
            HandleAllyCollision(collision.gameObject);
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collisionDamageImmunity)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleEnemyCollision(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Ally"))
        {
            HandleAllyCollision(collision.gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            CancelInvoke(nameof(CancelCollisionImmunity));
            collisionDamageImmunity = false;
            HandleEnemyCollision(other.gameObject);
        }

        if (other.CompareTag("Ally"))
        {
            CancelInvoke(nameof(CancelCollisionImmunity));
            collisionDamageImmunity = false;
            HandleAllyCollision(other.gameObject);
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (collisionDamageImmunity)
        {
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            HandleEnemyCollision(other.gameObject);
        }

        if (other.CompareTag("Ally"))
        {
            HandleAllyCollision(other.gameObject);
        }
    }


    public int GetCollisionDamage()
    {
        return collisionDamage;
    }

    protected abstract void UpdateHealthUI();
    protected abstract void PlayTakeDamageSound();


}
