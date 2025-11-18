using UnityEngine;
using System.Collections;

public class AircraftCarrier : MonoBehaviour
{
    [SerializeField] private float MovementSpeed;
    private Rigidbody2D rigidBody;

    [Header("Spawning")]
    [SerializeField] private GameObject jetPrefab;
    [SerializeField] private Transform jetSpawnPoint;
    [SerializeField] private int jetsPerWave = 3;
    [SerializeField] private float delayBetweenJets = 0.5f;
    [SerializeField] private float waveCooldown = 30f;

    private bool isSpawning = false;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isSpawning)
        {
            StartCoroutine(SpawnWave());
        }

        rigidBody.linearVelocity = transform.up * MovementSpeed;

        if (transform.rotation.z != 0)
        {
            transform.rotation = Quaternion.identity;
        }
    }

    private IEnumerator SpawnWave()
    {
        isSpawning = true;

        for (int i = 0; i < jetsPerWave; i++)
        {
            GameObject jetObj = Instantiate(jetPrefab, jetSpawnPoint.position, jetSpawnPoint.rotation);

            if (jetObj.TryGetComponent(out Jet jet))
            {
                jet.SetCarrier(this.transform);
            }

            yield return new WaitForSeconds(delayBetweenJets);
        }

        yield return new WaitForSeconds(waveCooldown);
        
        isSpawning = false;
    }
}
