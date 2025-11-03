using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("REFERENCES")]
    // [SerializeField] private Slider healthSlider;
    // [SerializeField] private TextMeshProUGUI healthText;
    private Rigidbody2D rb;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 100;
    private float currentHealth;
    private bool isDead = false;
    private float collisionDamage = 100;

    [Header("Damage Handling")]
    [SerializeField] private float damageImmunityDuration = 1f;
    [SerializeField] private GameObject explosionPrefab;
    private bool isInvincible = false;
    private SpriteRenderer sr;
    private Material defaultMaterial;

    [Header("Audio")]
    [SerializeField] private AudioClip takeDamageSound;
    [SerializeField] private AudioClip DieSound;
    private AudioSource audioSource;


    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();


        //healthSlider = GameObject.Find("Health Bar").GetComponent<Slider>();
        //healthText = GameObject.Find("Health Bar Text").GetComponent<TextMeshProUGUI>();


        currentHealth = maxHealth;


        //healthSlider.maxValue = maxHealth;
        //healthSlider.value = currentHealth;

        sr = GetComponent<SpriteRenderer>();
        defaultMaterial = sr.material;

        UpdateHealthUI();
    }


    private void Update()
    {
        UpdateHealthUI();
    }



    public void TakeDamage(float damage)
    {
        if (isInvincible || isDead) return;

        audioSource.PlayOneShot(takeDamageSound);

        float healthBeforeDamage = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0)
        {
            isDead = true;
            rb.simulated = false;
            audioSource.PlayOneShot(DieSound);
            StartCoroutine(SpawnExplosionsOverTime());
            Invoke(nameof(GameOver), 1.5f);
        }

        UpdateHealthUI();

        StartCoroutine(DamageFlashCoroutine());
    }


    private IEnumerator SpawnExplosionsOverTime()
    {
        for (int i = 0; i < 5; i++)
        {
            // Random position within the radius
            Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            // Instantiate the explosion
            GameObject explosion = Instantiate(explosionPrefab, spawnPosition, Quaternion.identity);

            // Try to get the Animator and auto-destroy when animation ends
            Animator anim = explosion.GetComponent<Animator>();
            if (anim != null)
            {
                // Get the length of the current animation clip (first state)
                AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);
                if (clipInfo.Length > 0)
                {
                    float clipLength = clipInfo[0].clip.length;
                    Destroy(explosion, clipLength);
                }
                else
                {
                    Destroy(explosion, 2f);
                }
            }
            else
            {
                Destroy(explosion, 2f);
            }

            yield return new WaitForSeconds(0.25f);
        }
    }


    private void GameOver()
    {
        /// TODO: restart game / reload scene / show end result
    }


    private IEnumerator DamageFlashCoroutine()
    {
        isInvincible = true;
        float flashDuration = damageImmunityDuration;
        float flashRate = 0.1f;
        float elapsed = 0f;

        Material flashMaterial = GameManager.Instance.GetDamageFlashMaterial();

        while (elapsed < flashDuration)
        {
            sr.material = flashMaterial;
            yield return new WaitForSeconds(flashRate / 2f);
            sr.material = defaultMaterial;
            yield return new WaitForSeconds(flashRate / 2f);
            elapsed += flashRate;
        }

        sr.material = defaultMaterial;
        isInvincible = false;
    }


    private void UpdateHealthUI()
    {
        //healthSlider.value = currentHealth;
        //healthText.text = Mathf.Ceil(currentHealth).ToString();
    }


    private void HandleEnemyCollision(GameObject enemyObject)
    {
        if (isInvincible || isDead) return;

        var enemyHealth = enemyObject.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(collisionDamage, false);
            TakeDamage(enemyHealth.GetCollisionDamage());
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        isInvincible = false;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleEnemyCollision(collision.gameObject);
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleEnemyCollision(collision.gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        isInvincible = false;

        if (other.CompareTag("Enemy"))
        {
            HandleEnemyCollision(other.gameObject);
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            HandleEnemyCollision(other.gameObject);
        }
    }
}
