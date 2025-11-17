using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private int damage;
    private float speed;
    private float lifeDuration = 15f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Destroy(gameObject);
            Debug.LogWarning("Enemy Projectile Prefab did not have rigidbody2D");
        }

        Destroy(gameObject, lifeDuration);
    }


    public void Initialize(float _speed, int _minDamage, int _maxDamage)
    {
        speed = _speed;
        damage = Random.Range(_minDamage, _maxDamage + 1);

        rb.linearVelocity = transform.right * speed;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") && !collision.CompareTag("Ally"))
        {
            Destroy(gameObject);
            return;
        }

        var player = collision.GetComponent<PlayerHealthBase>();
        player?.TakeDamage(damage);

        Destroy(gameObject);
    }
}
