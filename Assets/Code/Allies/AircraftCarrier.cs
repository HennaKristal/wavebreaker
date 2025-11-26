using UnityEngine;
using System.Collections;

public class AircraftCarrier : MonoBehaviour
{
    [SerializeField] private float MovementSpeed;
    private Rigidbody2D rigidBody;

    [Header("Jet Spawning")]
    [SerializeField] private GameObject jetPrefab;
    [SerializeField] private Transform jetSpawnPoint;
    [SerializeField] private int jetsPerWave = 3;
    [SerializeField] private float delayBetweenJets = 0.5f;
    [SerializeField] private float waveCooldown = 30f;
    private bool isSpawningJets = false;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rigidBody.linearVelocity = transform.up * MovementSpeed;

        if (transform.rotation.z != 0)
        {
            transform.rotation = Quaternion.identity;
        }

        if (!isSpawningJets)
        {
            StartCoroutine(SpawnJets());
        }
    }

    private IEnumerator SpawnJets()
    {
        isSpawningJets = true;

        for (int i = 0; i < jetsPerWave; i++)
        {
            GameObject jetGameobject = Instantiate(jetPrefab, jetSpawnPoint.position, jetSpawnPoint.rotation);
            Jet jet = jetGameobject.GetComponent<Jet>();
            jet?.SetCarrier(transform);

            yield return new WaitForSeconds(delayBetweenJets);
        }

        yield return new WaitForSeconds(waveCooldown);

        isSpawningJets = false;
    }
}
