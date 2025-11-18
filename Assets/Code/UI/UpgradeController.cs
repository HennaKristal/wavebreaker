using System.Collections;
using UnityEngine;
using TMPro;

public class UpgradeController : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeOption
    {
        public TextMeshProUGUI label;
        public int price;
        public TextMeshProUGUI priceLabel;
    }

    [Header("References")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private WaveSpawner waveSpawner;
    private Inventory inventory;

    [Header("Menu Options")]
    [SerializeField] private UpgradeOption[] options;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = new Color(0.3f, 0.9f, 1f);
    [SerializeField] private Color priceInsufficientColor = Color.red;

    [Header("Preview Prefabs")]
    [SerializeField] private GameObject destroyerPreviewPrefab;
    [SerializeField] private GameObject carrierPreviewPrefab;

    [Header("Audio")]
    [SerializeField] private AudioSource UIAudioSource;
    [SerializeField] private AudioClip insufficientFundsClip;

    private int row = 1;
    private float cooldown = 0.2f;
    private float nextInputTime;
    private float deadZone = 0.4f;
    private bool navigationEnabled = false;

    private void Start()
    {
        RefreshPriceLabels();
        inventory = GameManager.Instance.GetInventoryController();
    }

    public void OpenUpgradePanel()
    {
        Time.timeScale = 0f;
        navigationEnabled = false;
        StartCoroutine(EnableControlsRealtime(1f));
        row = 1;
        UpdateVisuals();
        upgradePanel.SetActive(true);
    }

    public void ReopenUpgradePanel()
    {
        Time.timeScale = 0f;
        navigationEnabled = false;
        StartCoroutine(EnableControlsRealtime(1f));
        upgradePanel.SetActive(true);
    }

    private IEnumerator EnableControlsRealtime(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        navigationEnabled = true;
        // reset input timing so immediate input isn't consumed
        nextInputTime = Time.unscaledTime + cooldown;
    }

    public void CloseUpgradePanel()
    {
        Time.timeScale = 1f;
        navigationEnabled = false;
        upgradePanel.SetActive(false);
    }

    private void Update()
    {
        if (!navigationEnabled) { return; }
        HandleMovement();
        HandleAction();
    }

    private void HandleMovement()
    {
        if (InputController.Instance == null) return;

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
            row = Mathf.Min(row, options.Length);
        }

        if (previousRow != row)
        {
            nextInputTime = Time.unscaledTime + cooldown;
            UpdateVisuals();
        }
    }

    private void HandleAction()
    {
        if (InputController.Instance.EnterPressed)
        {
            // If last option (Continue) selected
            if (row == options.Length)
            {
                CloseUpgradePanel();
                if (waveSpawner != null) waveSpawner.SpawnNextWave();
                return;
            }

            // Otherwise try to purchase selected upgrade
            UpgradeOption option = options[row - 1];
            if (option == null)
            {
                return;
            }

            if (!inventory.HasCoins(option.price))
            {
                // flash price label red and play error sound
                if (option.priceLabel != null)
                {
                    StartCoroutine(FlashPriceLabel(option.priceLabel, priceInsufficientColor, 1f));
                }

                UIAudioSource.PlayOneShot(insufficientFundsClip);

                return;
            }

            // can afford -> instantiate preview prefab and close panel
            GameObject prefabToSpawn = null;
            if (row == 1) prefabToSpawn = destroyerPreviewPrefab;
            else if (row == 2) prefabToSpawn = carrierPreviewPrefab;

            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity);
                CloseUpgradePanel();
            }
        }
    }

    private IEnumerator FlashPriceLabel(TextMeshProUGUI label, Color flashColor, float duration)
    {
        if (label == null) yield break;
        Color original = label.color;
        label.color = flashColor;
        yield return new WaitForSecondsRealtime(duration);
        label.color = original;
    }

    private void UpdateVisuals()
    {
        if (options == null || options.Length == 0) return;

        for (int i = 0; i < options.Length; i++)
        {
            var opt = options[i];
            if (opt == null) continue;

            Color col = normalColor;
            if (i == row - 1) col = highlightColor;

            if (opt.label != null) opt.label.color = col;
            if (opt.priceLabel != null) opt.priceLabel.color = col;
        }
    }

    private void RefreshPriceLabels()
    {
        if (options == null) return;
        foreach (var opt in options)
        {
            if (opt == null) continue;
            if (opt.priceLabel != null)
            {
                opt.priceLabel.text = opt.price.ToString() + "$";
            }
        }
    }
}
