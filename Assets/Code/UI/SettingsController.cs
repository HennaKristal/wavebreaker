using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class ToggleSlot
{
    public TextMeshProUGUI label;
    public Toggle toggle;
}


[System.Serializable]
public class AudioSlot
{
    public TextMeshProUGUI label;
    public Slider slider;
    public Image handle;
}


public class SettingsController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private MainMenuController mainMenuController;
    [SerializeField] private PauseController pauseController;

    [Header("Right Column Elements")]
    [SerializeField] private AudioSlot[] audioElements;

    [Header("Left Column Elements")]
    [SerializeField] private ToggleSlot[] toggleElements;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = new Color(0.3f, 0.9f, 1f);
    private int row = 1;
    private int column = 1;
    private float cooldown = 0.2f;
    private float nextInputTime;
    private float deadZone = 0.4f;
    private bool isEditingSlider = false;
    [HideInInspector] public bool navigationEnabled = false;


    public void OpenSettingsPanel()
    {
        Time.timeScale = 0f;
        row = 1;
        column = 1;
        settingsPanel.SetActive(true);
        LoadAccessibilitySettings();
        UpdateVisuals();
        StartCoroutine(ActivateNavigationDelayed(0.1f));
    }

    private IEnumerator ActivateNavigationDelayed(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (!GameManager.Instance.gameStarted)
        {
            mainMenuController.navigationEnabled = false;
        }

        navigationEnabled = true;
    }

    public void CloseSettingsPanel()
    {
     
        navigationEnabled = false;
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;

        if (!GameManager.Instance.gameStarted)
        {
            Invoke(nameof(ActivateMainMenuNavigation), 0.1f);
        }
        else
        {
            pauseController.OpenPausePanel(true);
        }
    }

    private void ActivateMainMenuNavigation()
    {
        mainMenuController.navigationEnabled = true;
    }

    private void Update()
    {
        if (!navigationEnabled)
        {
            return;
        }

        if (isEditingSlider)
        {
            HandleSliders();
        }
        else
        {
            HandleMovement();
        }

        HandleAction();
    }

    private void HandleSliders()
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

        nextInputTime = Time.unscaledTime + (cooldown / 2);

        if (move.x > deadZone / 2)
        {
            audioElements[row - 1].slider.value = Mathf.Clamp(audioElements[row - 1].slider.value + 0.05f, 0f, 1f);
        }
        else if (move.x < -deadZone / 2)
        {
            audioElements[row - 1].slider.value = Mathf.Clamp(audioElements[row - 1].slider.value - 0.05f, 0f, 1f);
        }
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
        int previousColumn = column;

        // Up
        if (move.y > deadZone)
        {
            row--;
            row = Mathf.Max(row, 1);
        }
        // Down
        else if (move.y < -deadZone)
        {
            if (column == 1)
            {
                row++;
                row = Mathf.Min(row, audioElements.Length);
            }
            else
            {
                row++;
                row = Mathf.Min(row, toggleElements.Length);
            }
        }
        // Right
        else if (move.x > deadZone)
        {
            column = 2;
            row = Mathf.Min(row, toggleElements.Length);
        }
        // Left
        else if (move.x < -deadZone)
        {
            column = 1;
            row = Mathf.Min(row, audioElements.Length);
        }

        if (previousRow != row || previousColumn != column)
        {
            nextInputTime = Time.unscaledTime + cooldown;
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        foreach (var element in audioElements)
        {
            element.label.color = normalColor;
        }

        foreach (var element in toggleElements)
        {
            element.label.color = normalColor;
        }

        if (column == 1)
        {
            audioElements[row - 1].label.color = highlightColor;
        }
        else
        {
            toggleElements[row - 1].label.color = highlightColor;
        }
    }

    private void HandleAction()
    {
        if (InputController.Instance.CancelPressed)
        {
            if (isEditingSlider)
            {
                isEditingSlider = false;
                audioElements[row - 1].handle.color = normalColor;
            }
            else
            {
                SaveAccessibilitySettings();
                AudioManager.Instance.SaveAudioSettings();
                CloseSettingsPanel();
            }
        }
        else if (InputController.Instance.EnterPressed)
        {
            if (isEditingSlider)
            {
                isEditingSlider = false;
                audioElements[row - 1].handle.color = normalColor;
            }
            else if (column == 1)
            {
                if (row == audioElements.Length)
                {
                    CloseSettingsPanel();
                }
                else
                {
                    isEditingSlider = true;
                    audioElements[row - 1].handle.color = highlightColor;
                }
            }
            else if (column == 2)
            {
                toggleElements[row - 1].toggle.isOn = !toggleElements[row - 1].toggle.isOn;
            }
        }
    }

    private void LoadAccessibilitySettings()
    {
        int i = 0;
        foreach (var toggleElement in toggleElements)
        {
            i++;
            int savedValue = PlayerPrefs.GetInt("accessibility_" + i, toggleElement.toggle.isOn ? 1 : 0);
            toggleElement.toggle.isOn = savedValue == 1;
        }
    }

    private void SaveAccessibilitySettings()
    {
        int i = 0;
        foreach (var toggleElement in toggleElements)
        {
            i++;
            PlayerPrefs.SetInt("accessibility_" + i, toggleElement.toggle.isOn ? 1 : 0);
        }
    }

    public void ReturnHovered()
    {
        column = 1;
        row = audioElements.Length;
        UpdateVisuals();
    }

    public void ReturnClicked()
    {
        CloseSettingsPanel();
    }
}
