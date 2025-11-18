using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifeDuration = 8f;
    private Rigidbody2D rigidBody;
    private int damage;
    private float speed;
    private bool isCritical = false;


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        if (rigidBody == null)
        {
            Destroy(gameObject);
            Debug.LogWarning("Projectile Prefab did not have rigidbody2D");
        }

        Destroy(gameObject, lifeDuration);
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

        rigidBody.linearVelocity = transform.right * speed;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            return;
        }

        var enemy = collision.GetComponent<EnemyHealthBase>();
        enemy?.TakeDamage(damage, isCritical);

        Destroy(gameObject);
    }
}
