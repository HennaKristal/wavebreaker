using UnityEngine;

public class ResourceAttractor : MonoBehaviour
{
    [SerializeField] private float attractForce = 2f;
    private Transform player;
    private bool playerInRadius = false;

    void Start()
    {
        player = GameManager.Instance.GetPlayerTransform();
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        if (!playerInRadius)
            return;

        transform.position = Vector2.MoveTowards(transform.position, player.position, attractForce * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRadius = true;
        }
    }
}
