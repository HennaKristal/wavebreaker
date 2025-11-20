using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

[System.Serializable]
public class Wave
{
    public List<EnemyType> enemyTypes;
    public List<CustomSpawn> customSpawns;
    public float spawnDuration;
    public float waveDuration;
    public string playSong = "";
}

[System.Serializable]
public class EnemyType
{
    public GameObject enemyPrefab;
    public int count;
}

[System.Serializable]
public class CustomSpawn
{
    public GameObject gameObject;
    public float activationTime;
    public bool keepUnactive;
    [HideInInspector] public bool hasBeenActivated;
}

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private Transform enemyContainer;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<Wave> waves;
    [SerializeField] private int currentWave = 0;
    private float waveTimer = 0f;
    private string currentSong = "";

    [Header("Between Wave UI")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private Transform FlagshipHQ;
    [SerializeField] private Transform player;
    [SerializeField] private ShopController shopController;
    private bool isTransitioningToUpgrade = false;
    public bool waitingForUpgrade = false;
    [SerializeField] private float transitionDuration = 2f;


    private void Start()
    {
        StartNextWave();
    }

    private void Update()
    {
        if (GameManager.Instance.gameEnded) return;

        if (waitingForUpgrade) return;

        waveTimer += Time.deltaTime;

        Wave wave = waves[currentWave - 1];

        // Start next wave if time ends or all enemies have been killed
        if (waveTimer >= wave.waveDuration || (enemyContainer.childCount == 0 && waveTimer >= wave.spawnDuration))
        {
            // If there are more waves left, pause and show upgrade UI between waves
            if (currentWave < waves.Count)
            {
                BeginUpgradeTransition();
            }
            else
            {
                GameManager.Instance.GameCompleted();
            }
        }

        // Custom spawns
        foreach (var customSpawn in wave.customSpawns)
        {
            if (!customSpawn.hasBeenActivated)
            {
                if (waveTimer >= customSpawn.activationTime)
                {
                    if (!customSpawn.keepUnactive)
                    {
                        customSpawn.gameObject.SetActive(true);
                    }

                    customSpawn.gameObject.transform.parent = enemyContainer;
                    customSpawn.hasBeenActivated = true;
                }
            }
        }
    }

    private void BeginUpgradeTransition()
    {
        if (isTransitioningToUpgrade || waitingForUpgrade) return;

        isTransitioningToUpgrade = true;

        // switch camera target to HQ
        if (cinemachineCamera != null && FlagshipHQ != null)
        {
            cinemachineCamera.Follow = FlagshipHQ;
            cinemachineCamera.LookAt = FlagshipHQ;
        }
        else
        {
            cinemachineCamera.Follow = player;
            cinemachineCamera.LookAt = player;
        }

        StartCoroutine(TransitionToUpgradeAndPause());
    }

    private IEnumerator TransitionToUpgradeAndPause()
    {
        yield return new WaitForSecondsRealtime(transitionDuration);

        waitingForUpgrade = true;
        isTransitioningToUpgrade = false;
        shopController.OpenShopPanel();
    }

    public void SpawnNextWave()
    {
        if (!waitingForUpgrade) return;

        if (cinemachineCamera != null)
        {
            if (player != null)
            {
                cinemachineCamera.Follow = player;
                cinemachineCamera.LookAt = player;
            }
        }

        waitingForUpgrade = false;

        StartNextWave();
    }

    private void StartNextWave()
    {
        currentWave++;
        waveTimer = 0f;

        if (currentWave <= waves.Count)
        {
            Wave wave = waves[currentWave - 1];
            StartCoroutine(SpawnWave(wave));

            if (!string.IsNullOrEmpty(wave.playSong) && wave.playSong != currentSong)
            {
                MusicManager.Instance.PlayMusic(wave.playSong);
            }
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        List<EnemySpawnTask> spawnTasks = new List<EnemySpawnTask>();

        // Gather all enemies to be spawned and assign random spawn times
        foreach (var enemyType in wave.enemyTypes)
        {
            for (int i = 0; i < enemyType.count; i++)
            {
                spawnTasks.Add(new EnemySpawnTask
                {
                    enemyPrefab = enemyType.enemyPrefab,
                    spawnTime = Random.Range(0, wave.spawnDuration)
                });
            }
        }

        // Sort by spawn time to ensure randomized but ordered timing
        spawnTasks.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));

        // Spawn enemies according to the sorted spawn times
        float previousSpawnTime = 0f;
        foreach (var task in spawnTasks)
        {
            float delay = task.spawnTime - previousSpawnTime;

            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            Vector3 spawnPosition = GetRandomPointOnCircleEdge(spawnRadius);
            GameObject enemy = Instantiate(task.enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.transform.parent = enemyContainer;
            previousSpawnTime = task.spawnTime;
        }
    }

    private Vector3 GetRandomPointOnCircleEdge(float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        return new Vector3(spawnPoint.position.x + Mathf.Cos(angle) * radius, spawnPoint.position.y + Mathf.Sin(angle) * radius, 0);
    }
}

// Helper class to store enemy spawn tasks
public class EnemySpawnTask
{
    public GameObject enemyPrefab;
    public float spawnTime;
}

// Extension method for shuffling a list
public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
