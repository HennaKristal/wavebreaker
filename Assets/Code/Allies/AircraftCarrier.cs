using UnityEngine;
using System.Collections;

public class AircraftCarrier : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private GameObject jetPrefab;
    [SerializeField] private Transform jetSpawnPoint;
    [SerializeField] private int jetsPerWave = 3;
    [SerializeField] private float delayBetweenJets = 0.5f;
    [SerializeField] private float waveCooldown = 30f;

    private bool isSpawning = false;

    private void Update()
    {
        if (!isSpawning)
        {
            StartCoroutine(SpawnWave());
        }
    }

    private IEnumerator SpawnWave()
    {
        isSpawning = true;

        for (int i = 0; i < jetsPerWave; i++)
        {
            Instantiate(jetPrefab, jetSpawnPoint.position, jetSpawnPoint.rotation);
            yield return new WaitForSeconds(delayBetweenJets);
        }

        yield return new WaitForSeconds(waveCooldown);

        isSpawning = false;
    }
}
