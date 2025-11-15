using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] Material playerDamageFlashMaterial;
    [SerializeField] Material enemyDamageFlashMaterial;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Light2D globalLight;
    [SerializeField] private GameObject bossHealthBar;
    [SerializeField] private Slider bossHealthBarSlider;
    [SerializeField] private TextMeshProUGUI bossHealthBarText;

    private static GameManager _instance;
    public static GameManager Instance => _instance;
    private Fading fading;
    private Coroutine sceneRoutine;
    public bool gameStarted = false;


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

    public Inventory GetInventory()
    {
        return inventory;
    }

    public Light2D GetGlobalLight()
    {
        return globalLight;
    }

    public GameObject GetBossHealthBar()
    {
        return bossHealthBar;
    }

    public Slider GetBossHealthBarSlider()
    {
        return bossHealthBarSlider;
    }

    public TextMeshProUGUI GetBossHealthBarText()
    {
        return bossHealthBarText;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fading != null)
        {
            fading.StartFadeIn(2f);
        }
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
