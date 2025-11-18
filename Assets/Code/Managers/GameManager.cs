using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    [Header("REFERENCES")]
    [SerializeField] Material playerDamageFlashMaterial;
    [SerializeField] Material enemyDamageFlashMaterial;
    [SerializeField] Material allyDamageFlashMaterial;
    private Transform playerTransform;
    private Transform flagshipHQTransform;
    private Inventory inventoryController;
    private AudioSource UIAudioSource;
    private Fading fading;
    private Coroutine sceneRoutine;
    public bool gameStarted = false;
    public bool gameEnded = false;
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

    public Material GetAllyDamageFlashMaterial()
    {
        return allyDamageFlashMaterial;
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public Transform GetFlagshipHQTransform()
    {
        return flagshipHQTransform;
    }

    public Inventory GetInventoryController()
    {
        return inventoryController;
    }

    public AudioSource GetUIAudioSource()
    {
        return UIAudioSource;
    }

    public void GameOver()
    {
        gameEnded = true;
        LoadSceneByName("Game");
    }

    public void GameCompleted()
    {
        gameEnded = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fading != null)
        {
            fading.StartFadeIn(2f);
        }

        gameStarted = false;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        flagshipHQTransform = GameObject.Find("FlagShipHQ").transform;
        UIAudioSource = GameObject.Find("UIAudioSource").GetComponent<AudioSource>();
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
