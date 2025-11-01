using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("STATS")]
    [SerializeField] private float health = 50;
    [SerializeField] private float collisionDamage = 100;


    [Header("DEATH")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionDuration = 0f;
    [SerializeField] private int explosionAmount = 1;
    [SerializeField] private float explosionRadius = 0f;

    [Header("REFERENCES")]
    [SerializeField] private GameObject damageNumberPrefab;
    private Collider2D enemyCollider;
    private bool isDead = false;


    private void Start()
    {
        enemyCollider = GetComponent<Collider2D>();
    }


    public void TakeDamage(float damageAmount, bool isCritical)
    {
        if (isDead) { return; }

        damageAmount = Mathf.Round(damageAmount);
        health -= damageAmount;

        if (damageNumberPrefab != null)
        {
            GameObject damageNumber = Instantiate(damageNumberPrefab, transform.position, Quaternion.identity);
            damageNumber.GetComponent<DamageNumber>().Initialize((int)damageAmount, isCritical);
        }

        if (health <= 0 && !isDead)
        {
            isDead = true;
            enemyCollider.enabled = false;

            GiveKillRewards();

            if (explosionPrefab != null && explosionAmount > 0)
            {
                StartCoroutine(SpawnExplosionsOverTime());
            }

            if (explosionPrefab != null && explosionAmount > 0)
            {
                Destroy(gameObject, explosionDuration + 0.1f);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }


    private void GiveKillRewards()
    {
        // TODO: Spawn Rewards
    }


    private IEnumerator SpawnExplosionsOverTime()
    {
        float interval = (explosionAmount > 1) ? explosionDuration / (explosionAmount - 1) : 0f;

        for (int i = 0; i < explosionAmount; i++)
        {
            // Random position within the radius
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * explosionRadius;
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
                    Destroy(explosion, clipLength + 1f);
                }
                else
                {
                    Destroy(explosion, 3f);
                }
            }
            else
            {
                Destroy(explosion, 3f);
            }

            yield return new WaitForSeconds(interval);
        }
    }

    public float GetCollisionDamage()
    {
        return collisionDamage;
    }
}
