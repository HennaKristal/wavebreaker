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

    private enum ResourceType { Coin }
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int amount = 1;

    [Header("Audio")]
    private AudioSource pickupAudioSource;
    [SerializeField] private AudioClip[] pickupAudioClips;

    private void Start()
    {
        inventory = GameManager.Instance.GetInventoryController();
        pickupAudioSource = GameObject.Find("PickupAudioSource")?.GetComponent<AudioSource>();
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
        if (pickupAudioSource == null)
            return;

        if (pickupAudioClips == null || pickupAudioClips.Length == 0)
            return;

        int randomIndex = Random.Range(0, pickupAudioClips.Length);
        AudioClip selectedClip = pickupAudioClips[randomIndex];
        pickupAudioSource.PlayOneShot(selectedClip);
    }
}
