using System.Collections;
using TMPro;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private SettingsController settingsController;

    [Header("Buttons")]
    [SerializeField] private TextMeshProUGUI[] buttons;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = new Color(0.3f, 0.9f, 1f);

    private int row = 1;
    private float cooldown = 0.2f;
    private float nextInputTime;
    private float deadZone = 0.4f;
    private bool navigationEnabled = false;



    public void OpenPausePanel(bool cameFromSettings = false)
    {
        Time.timeScale = 0f;

        row = 1;
        if (cameFromSettings)
        {
            row = 2;
        }

        pausePanel.SetActive(true);
        UpdateVisuals();
        StartCoroutine(ActivateNavigationDelayed(0.1f));
    }

    private IEnumerator ActivateNavigationDelayed(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        navigationEnabled = true;
    }

    public void ClosePausePanel()
    {
        navigationEnabled = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }


    private void Update()
    {
        if (!GameManager.Instance.gameStarted || settingsController.navigationEnabled)
        {
            return;
        }

        if (!navigationEnabled)
        {
            if (InputController.Instance.PausePressed)
            {
                OpenPausePanel();
            }

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
            nextInputTime = Time.unscaledTime;
            return;
        }

        if (Time.unscaledTime < nextInputTime)
        {
            return;
        }

        int previousRow = row;

        if (move.y > deadZone)
        {
            row--;
            row = Mathf.Max(row, 1);
        }
        else if (move.y < -deadZone)
        {
            row++;
            row = Mathf.Min(row, buttons.Length);
        }

        if (previousRow != row)
        {
            nextInputTime = Time.unscaledTime + cooldown;
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        foreach (var element in buttons)
        {
            element.color = normalColor;
        }

        buttons[row - 1].color = highlightColor;
    }

    private void HandleAction()
    {
        if (InputController.Instance.CancelPressed)
        {
            ClosePausePanel();
        }
        else if (InputController.Instance.EnterPressed)
        {
            ClosePausePanel();

            switch (row)
            {
                case 1: break;
                case 2: settingsController.OpenSettingsPanel(); break;
                case 3: GameManager.Instance.LoadSceneByName("Game"); break;
            }
        }
    }

    public void ResumeHovered()
    {
        row = 1;
        UpdateVisuals();
    }

    public void SettingsHovered()
    {
        row = 2;
        UpdateVisuals();
    }

    public void RestartHovered()
    {
        row = 3;
        UpdateVisuals();
    }

    public void ResumeClicked()
    {
        ClosePausePanel();
    }

    public void SettingsClicked()
    {
        ClosePausePanel();
        settingsController.OpenSettingsPanel();
    }

    public void RestartClicked()
    {
        ClosePausePanel();
        GameManager.Instance.LoadSceneByName("Game");
    }
}
