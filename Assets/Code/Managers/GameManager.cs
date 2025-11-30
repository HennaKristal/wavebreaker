using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
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

   protected override void Awake()
    {
        base.Awake();

        fading = GetComponent<Fading>();
        if (fading != null)
        {
            fading.StartFadeIn(2f);
        }

        gameStarted = false;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerTransform.gameObject.SetActive(false);
        flagshipHQTransform = GameObject.Find("FlagShipHQ").transform;
        flagshipHQTransform.gameObject.SetActive(false);
        UIAudioSource = GameObject.Find("UIAudioSource").GetComponent<AudioSource>();
        inventoryController = GameObject.Find("InventoryController").GetComponent<Inventory>();
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

    public void StartGame()
    {
        gameStarted = true;
        gameEnded = false;
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
