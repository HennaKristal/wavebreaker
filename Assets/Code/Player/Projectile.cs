using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private int damage;
    private float speed;
    private bool isCritical = false;
    private bool hasProcessedCollision = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float _speed, int _minDamage, int _maxDamage, float _criticalChance, float _criticalDamageMultiplier)
    {
        speed = _speed;
        damage = Random.Range(_minDamage, _maxDamage + 1);
        isCritical = (Random.Range(1, 101) <= _criticalChance);

        if (isCritical)
        {
            damage = Mathf.RoundToInt(damage * _criticalDamageMultiplier);
        }

        if (rb != null)
        {
            rb.linearVelocity = transform.right * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasProcessedCollision) return;
        hasProcessedCollision = true;

        if (!collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            return;
        }

        var enemy = collision.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, isCritical);
        }

        Destroy(gameObject);
    }
}
