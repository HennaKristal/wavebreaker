using System.Collections;
using UnityEngine;
using TMPro;

public class ShopController : MonoBehaviour
{
    [System.Serializable]
    public class PurchaseOption
    {
        public TextMeshProUGUI label;
        public int price;
        public GameObject previewPrefab;
        public TextMeshProUGUI priceLabel;
    }

    [Header("References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private WaveSpawner waveSpawner;
    private Inventory inventory;

    [Header("Menu purchaseOptionList")]
    [SerializeField] private PurchaseOption[] purchaseOptionList;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = new Color(0.3f, 1f, 1f);
    [SerializeField] private Color priceInsufficientColor = Color.red;

    [Header("Audio")]
    [SerializeField] private AudioSource UIAudioSource;
    [SerializeField] private AudioClip errorAudioClip;
    [SerializeField] private AudioClip purchaseAudioClip;
    [SerializeField] private AudioClip buttonHoverSound;
    [SerializeField] private AudioClip buttonClickSound;

    private int row = 1;
    private float cooldown = 0.2f;
    private float nextInputTime;
    private float deadZone = 0.4f;
    private bool navigationEnabled = false;
    public bool isPlacingShopItems = false;


    private void Start()
    {
        RefreshPriceLabels();
        inventory = GameManager.Instance.GetInventoryController();
    }

    private void RefreshPriceLabels()
    {
        foreach (var purchase in purchaseOptionList)
        {
            if (purchase.priceLabel != null)
            {
                purchase.priceLabel.text = purchase.price.ToString() + "$";
            }
        }
    }

    public void OpenShopPanel()
    {
        Time.timeScale = 0f;
        StartCoroutine(EnableControlsRealtime(0.1f));
        shopPanel.SetActive(true);
        row = 1;
        UpdateVisuals(playSound: false);

    }

    public void ReopenShopPanel()
    {
        StartCoroutine(EnableControlsRealtime(0.1f));
        isPlacingShopItems = false;
        shopPanel.SetActive(true);
    }

    private IEnumerator EnableControlsRealtime(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        navigationEnabled = true;

        nextInputTime = Time.unscaledTime + cooldown;
    }

    public void CloseShopPanel()
    {
        navigationEnabled = false;
        shopPanel.SetActive(false);
    }

    private void Update()
    {
        if (!navigationEnabled) { return; }
        HandleMovement();
        HandleAction();
    }

    private void HandleMovement()
    {
        Vector2 move = InputManager.Instance.Move;

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
            row = Mathf.Min(row, purchaseOptionList.Length);
        }

        if (previousRow != row)
        {
            nextInputTime = Time.unscaledTime + cooldown;
            UpdateVisuals();
        }
    }

    private void HandleAction()
    {
        if (InputManager.Instance.EnterPressed)
        {
            // If last option (Continue) selected
            if (row == purchaseOptionList.Length)
            {
                ClickContinueButton();
                return;
            }

            PlacePreviewPrefab();
        }
    }

    private void PlacePreviewPrefab()
    {
        PurchaseOption purchase = purchaseOptionList[row - 1];
        if (purchase == null) { return; }

        if (!inventory.HasCoins(purchase.price))
        {
            if (purchase.priceLabel != null)
            {
                StartCoroutine(FlashPriceLabel(purchase.priceLabel, priceInsufficientColor, 1f));
            }

            PlayErrorSound();

            return;
        }

        SpawnPreviewPrefab(purchase.previewPrefab);
    }

    private void SpawnPreviewPrefab(GameObject prefabToSpawn)
    {
        Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity);
        isPlacingShopItems = true;
        CloseShopPanel();
    }

    private IEnumerator FlashPriceLabel(TextMeshProUGUI label, Color flashColor, float duration)
    {
        Color original = label.color;
        label.color = flashColor;
        yield return new WaitForSecondsRealtime(duration);
        label.color = original;
    }

    private void UpdateVisuals(bool playSound = true)
    {
        if (playSound)
            PlayHoverSound();

        foreach (var purchase in purchaseOptionList)
            purchase.label.color = normalColor;

        purchaseOptionList[row - 1].label.color = highlightColor;
    }

    public void HoverPurchaseDestroyerButton()
    {
        if (row == 1)
            return;

        row = 1;
        UpdateVisuals();
    }

    public void HoverPurchaseAircraftCarrierButton()
    {
        if (row == 2)
            return;

        row = 2;
        UpdateVisuals();
    }

    public void HoverContinueButton()
    {
        if (row == 3)
            return;

        row = 3;
        UpdateVisuals();
    }

    public void ClickPurchaseDestroyerButton()
    {
        PlayClickSound();
        row = 1;
        PlacePreviewPrefab();
    }

    public void ClickPurchaseAircraftCarrierButton()
    {
        PlayClickSound();
        row = 2;
        PlacePreviewPrefab();
    }

    public void ClickContinueButton()
    {
        Time.timeScale = 1f;
        PlayClickSound();
        CloseShopPanel();
        if (waveSpawner != null) waveSpawner.SpawnNextWave();
    }

    public void PlayHoverSound()
    {
        UIAudioSource.PlayOneShot(buttonHoverSound);
    }

    public void PlayClickSound()
    {
        UIAudioSource.PlayOneShot(buttonClickSound);
    }

    public void PlayErrorSound()
    {
        UIAudioSource.PlayOneShot(errorAudioClip);
    }
}
