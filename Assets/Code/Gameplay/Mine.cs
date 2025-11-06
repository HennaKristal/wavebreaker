using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private int damage = 100;
    [SerializeField] private float explosionRadius = 1f;
    private bool hasExploded = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded) return;

        if (collision.CompareTag("Enemy") || collision.CompareTag("Player") || collision.CompareTag("PlayerBullet") || collision.CompareTag("EnemyBullet"))
        {
            hasExploded = true;
            DealAreaDamage();
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }


    private void DealAreaDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                var enemy = hit.GetComponent<EnemyHealthBase>();
                enemy?.TakeDamage(damage, false);
            }
            else if (hit.CompareTag("Player"))
            {
                var player = hit.GetComponent<PlayerController>();
                player?.TakeDamage(damage);
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
