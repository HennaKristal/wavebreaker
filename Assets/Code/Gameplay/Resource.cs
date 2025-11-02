using UnityEngine;

public class Resource : MonoBehaviour
{
    private Inventory inventory;

    private enum ResourceType { Coin, Material }
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int amount = 1;

    [Header("Magnet Settings")]
    [SerializeField] private float attractRadius = 3f;
    [SerializeField] private float attractForce = 10f;

    private Transform player;
    private Rigidbody2D rb;

    private void Start()
    {
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= attractRadius)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.AddForce(direction * attractForce);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switch (resourceType)
            {
                case ResourceType.Coin:
                    inventory?.AddCoins(amount);
                    break;
                case ResourceType.Material:
                    inventory?.AddMaterials(amount);
                    break;
            }

            Destroy(gameObject);
        }
    }
}



[System.Serializable]
public class ResourceDrop
{
    public GameObject prefab;
    public int amount = 1;
}