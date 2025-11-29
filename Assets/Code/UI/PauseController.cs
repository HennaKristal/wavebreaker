using System.Collections;
using TMPro;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private SettingsController settingsController;
    [SerializeField] private ShopController shopController;
    [SerializeField] private GameObject tutorialPanel;
    private bool wasTutorialOpen = false;

    [Header("Buttons")]
    [SerializeField] private TextMeshProUGUI[] buttons;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = new Color(0.3f, 0.9f, 1f);

    [Header("Audio")]
    [SerializeField] private AudioSource UIAudioSource;
    [SerializeField] private AudioClip openPanelAudioClip;
    [SerializeField] private AudioClip buttonHoverSound;
    [SerializeField] private AudioClip buttonClickSound;
    private AudioSource musicAudioSource;
    private AudioSource ambienceAudioSource;
    private float previousMusicVolume = 1f;
    private float previousAmbienceVolume = 1f;

    private int row = 1;
    private float cooldown = 0.2f;
    private float nextInputTime;
    private float deadZone = 0.4f;
    private bool navigationEnabled = false;

    private void Start()
    {
        musicAudioSource = GameObject.Find("MusicManager")?.GetComponent<AudioSource>();
        ambienceAudioSource = GameObject.Find("AmbienceAudioSource")?.GetComponent<AudioSource>();
    }

    public void OpenPausePanel(bool cameFromSettings = false)
    {
        if (shopController.isPlacingShopItems || GameManager.Instance.gameEnded)
            return;

        wasTutorialOpen = tutorialPanel.activeSelf;
        tutorialPanel.SetActive(false);

        PlayOpenPanelSound();

        if (musicAudioSource != null)
        {
            previousMusicVolume = musicAudioSource.volume;
            musicAudioSource.volume *= 0.25f;
        }

        if (ambienceAudioSource != null)
        {
            previousAmbienceVolume = ambienceAudioSource.volume;
            ambienceAudioSource.volume *= 0.25f;
        }

        Time.timeScale = 0f;

        row = 1;
        if (cameFromSettings)
        {
            row = 2;
        }

        pausePanel.SetActive(true);
        UpdateVisuals(playSound: false);
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
        tutorialPanel.SetActive(wasTutorialOpen);

        if (musicAudioSource != null)
        {
            musicAudioSource.volume = previousMusicVolume;
        }

        if (ambienceAudioSource != null)
        {
            ambienceAudioSource.volume = previousAmbienceVolume;
        }

        if (!shopController.isPlacingShopItems)
        {
            Time.timeScale = 1f;
        }
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

    private void UpdateVisuals(bool playSound = true)
    {
        if (playSound)
        {
            PlayHoverSound();
        }

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
            PlayClickSound();
            ClosePausePanel();
            return;
        }

        if (InputController.Instance.EnterPressed)
        {
            PlayClickSound();
            ClosePausePanel();

            switch (row)
            {
                case 1: break;
                case 2: settingsController.OpenSettingsPanel(); break;
                case 3: GameManager.Instance.GameOver(); break;
            }
        }
    }

    public void ResumeHovered()
    {
        if (row == 1)
            return;

        row = 1;
        UpdateVisuals();
    }

    public void SettingsHovered()
    {
        if (row == 2)
            return;

        row = 2;
        UpdateVisuals();
    }

    public void RestartHovered()
    {
        if (row == 3)
            return;

        row = 3;
        UpdateVisuals();
    }

    public void ResumeClicked()
    {
        PlayClickSound();
        ClosePausePanel();
    }

    public void SettingsClicked()
    {
        PlayClickSound();
        ClosePausePanel();
        settingsController.OpenSettingsPanel();
    }

    public void RestartClicked()
    {
        PlayClickSound();
        ClosePausePanel();
        GameManager.Instance.GameOver();
    }

    public void PlayOpenPanelSound()
    {
        UIAudioSource.PlayOneShot(openPanelAudioClip);
    }

    public void PlayHoverSound()
    {
        UIAudioSource.PlayOneShot(buttonHoverSound);
    }

    public void PlayClickSound()
    {
        UIAudioSource.PlayOneShot(buttonClickSound);
    }
}
