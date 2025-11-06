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
    public static GameManager Instance => _instance ??= FindFirstObjectByType<GameManager>();
    private InputController inputController;
    private Fading fading;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        inputController = GetComponent<InputController>();
        fading = GetComponent<Fading>();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        fading.StartFadeIn(2f);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }

    public InputController GetInputController()
    {
        return inputController;
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

    public void LoadSceneByName(string sceneName) => StartCoroutine(ChangeScene(sceneName));
    private IEnumerator ChangeScene(string sceneName)
    {
        fading.StartFadeOut(2f);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneName);
    }
}
