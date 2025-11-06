using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    private Rigidbody2D rb;
    [SerializeField] private GameObject damageNumberPrefab;

    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int collisionDamage = 100;
    [SerializeField] private int currentHealth;
    private bool isDead = false;

    [Header("Damage Handling")]
    [SerializeField] private GameObject explosionPrefab;
    private SpriteRenderer sr;
    private Material defaultMaterial;
    private Material flashMaterial;
    private float collisionDamageImmunityDuration = 1f;
    private bool collisionDamageImmunity = false;

    [Header("Audio")]
    [SerializeField] private AudioClip takeDamageSound;
    [SerializeField] private AudioClip DieSound;
    private AudioSource audioSource;


    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        sr = GetComponent<SpriteRenderer>();
        defaultMaterial = sr.material;
        flashMaterial = GameManager.Instance.GetPlayerDamageFlashMaterial();

        UpdateHealthUI();
    }


    private void Update()
    {
        UpdateHealthUI();
    }


    public void TakeDamage(int damage)
    {
        if (isDead) return;

        audioSource.PlayOneShot(takeDamageSound);

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
            rb.simulated = false;
            audioSource.PlayOneShot(DieSound);
            StartCoroutine(SpawnExplosionsOverTime());
            Invoke(nameof(GameOver), 1.5f);
        }

        UpdateHealthUI();

        sr.material = flashMaterial;
        StartCoroutine(RevertDamageFlash());
    }


    private IEnumerator RevertDamageFlash()
    {
        yield return new WaitForSeconds(0.1f);
        sr.material = defaultMaterial;
    }


    private IEnumerator SpawnExplosionsOverTime()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            Instantiate(explosionPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(0.25f);
        }
    }


    private void UpdateHealthUI()
    {
        healthSlider.value = currentHealth;
        healthText.text = Mathf.Ceil(currentHealth).ToString() + " / " + maxHealth.ToString();
    }


    private void HandleEnemyCollision(GameObject enemyObject)
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


    private void CancelCollisionImmunity()
    {
        collisionDamageImmunity = false;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            CancelInvoke(nameof(CancelCollisionImmunity));
            collisionDamageImmunity = false;

            HandleEnemyCollision(collision.gameObject);
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !collisionDamageImmunity)
        {
            HandleEnemyCollision(collision.gameObject);
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
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !collisionDamageImmunity)
        {
            HandleEnemyCollision(other.gameObject);
        }
    }


    private void GameOver()
    {
        /// TODO: restart game / reload scene / show end result
    }
}
