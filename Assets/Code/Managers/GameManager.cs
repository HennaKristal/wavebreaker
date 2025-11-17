using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    [Header("REFERENCES")]
    [SerializeField] Material playerDamageFlashMaterial;
    [SerializeField] Material enemyDamageFlashMaterial;
    private Transform playerTransform;
    private Light2D globalLight;
    private Inventory inventoryController;
    private Fading fading;
    private Coroutine sceneRoutine;
    public bool gameStarted = false;
    public bool bossReached = false;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        fading = GetComponent<Fading>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }



    private void OnDestroy()
    {
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }

    public Material GetPlayerDamageFlashMaterial()
    {
        return playerDamageFlashMaterial;
    }

    public Material GetEnemyDamageFlashMaterial()
    {
        return enemyDamageFlashMaterial;
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public Inventory GetInventoryController()
    {
        return inventoryController;
    }

    public Light2D GetGlobalLight()
    {
        return globalLight;
    }

    public void GameOver()
    {
        LoadSceneByName("Game");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fading != null)
        {
            fading.StartFadeIn(2f);
        }

        gameStarted = false;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        globalLight = GameObject.Find("Global Light 2D").GetComponent<Light2D>();
        inventoryController = GameObject.Find("InventoryController").GetComponent<Inventory>();
    }

    public void LoadSceneByName(string sceneName)
    {
        if (sceneRoutine != null)
        {
            StopCoroutine(sceneRoutine);
        }

        sceneRoutine = StartCoroutine(ChangeScene(sceneName));
    }

    private IEnumerator ChangeScene(string sceneName)
    {
        if (fading != null)
        {
            fading.StartFadeOut(2f);
        }

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneName);
    }
}
