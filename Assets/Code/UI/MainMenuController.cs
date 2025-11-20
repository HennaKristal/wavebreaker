using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [System.Serializable]
    public struct Credit
    {
        public Image image;
        public string url;
    }

    [Header("References")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private WaveSpawner waveSpawner;
    [SerializeField] private Transform flagshipHQ;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private SettingsController settingsController;
    [SerializeField] private GameObject healthbarUI;
    [SerializeField] private GameObject flagshipHPUI;
    [SerializeField] private GameObject resourcesUI;
    [SerializeField] private GameObject controlHintsUI;

    [Header("Tutorial UI")]
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private TextMeshProUGUI tutorialText;


    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI[] gameButtons;
    [SerializeField] private Credit[] credits;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = new Color(0.3f, 0.9f, 1f);

    private enum MainMenuColumn { Credits, Menu };
    private MainMenuColumn column = MainMenuColumn.Menu;
    private int row = 1;
    private float cooldown = 0.2f;
    private float nextInputTime;
    private float deadZone = 0.4f;
    [HideInInspector] public bool navigationEnabled = false;


    private void Start()
    {
        UpdateVisuals();
        navigationEnabled = true;

        MusicManager.Instance.PlayMusic("BattleTheme");
    }

    private void Update()
    {
        if (!navigationEnabled)
        {
            return;
        }

        HandleMovement();
        HandleAction();
    }

    private void HandleMovement()
    {
        Vector2 move = InputController.Instance.Move;

        if (move == Vector2.zero)
        {
            nextInputTime = Time.time;
            return;
        }

        if (Time.time < nextInputTime)
        {
            return;
        }

        int previousRow = row;
        MainMenuColumn previousColumn = column;

        // Up
        if (move.y > deadZone)
        {
            row--;
            row = Mathf.Max(row, 1);
        }
        // Down
        else if (move.y < -deadZone)
        {
            if (column == MainMenuColumn.Credits)
            {
                row++;
                row = Mathf.Min(row, credits.Length);
            }
            else
            {
                row++;
                row = Mathf.Min(row, gameButtons.Length);
            }
        }
        // Right
        else if (move.x > deadZone)
        {
            column = MainMenuColumn.Menu;
            row = Mathf.Min(row, gameButtons.Length);
        }
        // Left
        else if (move.x < -deadZone)
        {
            column = MainMenuColumn.Credits;
            row = Mathf.Min(row, credits.Length);
        }

        if (previousRow != row || previousColumn != column)
        {
            nextInputTime = Time.time + cooldown;
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        foreach (var element in gameButtons)
        {
            element.color = normalColor;
        }

        foreach (var element in credits)
        {
            element.image.color = normalColor;
        }

        if (column == MainMenuColumn.Credits)
        {
            credits[row - 1].image.color = highlightColor;
        }
        else
        {
            gameButtons[row - 1].color = highlightColor;
        }
    }

    private void HandleAction()
    {
        if (InputController.Instance.EnterPressed)
        {

            if (column == MainMenuColumn.Menu)
            {
                switch (row)
                {
                    case 1: StartGame(); break;
                    case 2: settingsController.OpenSettingsPanel(); break;
                }
            }
            else
            {
                GameManager.Instance.OpenLink(credits[row - 1].url);
            }
        }
    }

    private void StartGame()
    {
        GameManager.Instance.gameStarted = true;

        mainMenuPanel.SetActive(false);

        healthbarUI.SetActive(true);
        flagshipHPUI.SetActive(true);
        resourcesUI.SetActive(true);
        controlHintsUI.SetActive(true);

        Tutorial1();

        navigationEnabled = false;
        this.enabled = false;
    }

    private void Tutorial1()
    {
        tutorialUI.SetActive(true);
        player.gameObject.SetActive(true);
        cinemachineCamera.Follow = player;
        cinemachineCamera.LookAt = player;
        tutorialText.text = "You are the commander of this battleship.";
        Invoke(nameof(Tutorial2), 4f);
    }

    private void Tutorial2()
    {
        flagshipHQ.gameObject.SetActive(true);
        cinemachineCamera.Follow = flagshipHQ;
        cinemachineCamera.LookAt = flagshipHQ;
        tutorialText.text = "Your mission is to defend the fleet's capital flagship.";
        Invoke(nameof(Tutorial3), 5f);
    }

    private void Tutorial3()
    {
        cinemachineCamera.Follow = player;
        cinemachineCamera.LookAt = player;
        waveSpawner.enabled = true;
        tutorialText.text = "The enemy waves are approaching. Good luck!";
        Invoke(nameof(Tutorial4), 4f);
    }

    private void Tutorial4()
    {
        tutorialUI.SetActive(false);
    }

    public void DeveloperCreditHovered()
    {
        column = MainMenuColumn.Credits;
        row = 1;
        UpdateVisuals();
    }

    public void MusicCreditHovered()
    {
        column = MainMenuColumn.Credits;
        row = 2;
        UpdateVisuals();
    }

    public void StartGameHovered()
    {
        column = MainMenuColumn.Menu;
        row = 1;
        UpdateVisuals();
    }

    public void SettingsHovered()
    {
        column = MainMenuColumn.Menu;
        row = 2;
        UpdateVisuals();
    }

    public void DeveloperCreditClicked()
    {
        GameManager.Instance.OpenLink(credits[row - 1].url);
    }

    public void MusicCreditClicked()
    {
        GameManager.Instance.OpenLink(credits[row - 1].url);
    }

    public void StartGameClicked()
    {
        StartGame();
    }

    public void SettingsClicked()
    {
        settingsController.OpenSettingsPanel();
    }
}
