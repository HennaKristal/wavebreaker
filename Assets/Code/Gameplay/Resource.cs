using UnityEngine;


[System.Serializable]
public class ResourceDrop
{
    public GameObject prefab;
    public int amount = 1;
}


public class Resource : MonoBehaviour
{
    private Inventory inventory;
    private Transform player;

    private enum ResourceType { Coin }
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int amount = 1;

    [Header("MAGNETISM")]
    [SerializeField] private float attractRadius = 3f;
    [SerializeField] private float attractForce = 2f;

    [Header("Audio")]
    private AudioSource pickupAudioSource;
    [SerializeField] private AudioClip pickupAudioClip;

    private void Start()
    {
        player = GameManager.Instance.GetPlayerTransform();
        inventory = GameManager.Instance.GetInventoryController();
        pickupAudioSource = GameObject.Find("PickupAudioSource")?.GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, player.position) <= attractRadius)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, attractForce * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayPickupSound();

            switch (resourceType)
            {
                case ResourceType.Coin:
                    inventory?.AddCoins(amount);
                    break;
            }

            Destroy(gameObject);
        }
    }

    public void PlayPickupSound()
    {
        pickupAudioSource.PlayOneShot(pickupAudioClip);
    }
}
