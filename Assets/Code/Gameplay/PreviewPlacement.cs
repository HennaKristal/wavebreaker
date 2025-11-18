using UnityEngine;

public class PreviewPlacement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject prefab;
    private UpgradeController upgradeController;
    private Inventory inventory;
    private Transform flagshipHQTransform;
    private SpriteRenderer spriteRenderer;

    [Header("Movement")]
    [SerializeField] private float inputMoveSpeed = 3f;
    [SerializeField] private float mouseMoveThreshold = 0.01f;
    private Vector3 lastMousePosition;

    [Header("Price")]
    [SerializeField] private int price = 2;

    [Header("Audio")]
    [SerializeField] private AudioClip errorAudioClip;
    private AudioSource UIAudioSource;

    // layer mask for allies
    private int allyLayerMask;

    private void Start()
    {
        inventory = GameManager.Instance.GetInventoryController();

        UIAudioSource = GameManager.Instance.GetUIAudioSource();

        flagshipHQTransform = GameManager.Instance.GetFlagshipHQTransform();
        transform.position = flagshipHQTransform.position;

        lastMousePosition = Input.mousePosition;

        upgradeController = GameObject.Find("UpgradeController")?.GetComponent<UpgradeController>();
        if (upgradeController == null)
        {
            Debug.LogError("PreviewPlacement: UpgradeController not found in scene.");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        allyLayerMask = LayerMask.GetMask("Ally");
    }

    private void Update()
    {
        HandleMovement();
        HandleActions();
    }

    private void HandleMovement()
    {
        // Follow mouse if it's moving; otherwise follow input
        Vector3 currentMousePos = Input.mousePosition;
        bool mouseMoved = (currentMousePos - lastMousePosition).sqrMagnitude > (mouseMoveThreshold * mouseMoveThreshold);

        if (mouseMoved && Camera.main != null)
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(currentMousePos);
            world.z = 0f;
            transform.position = world;
            lastMousePosition = currentMousePos;
        }
        else
        {
            if (InputController.Instance != null)
            {
                Vector2 inputVector = InputController.Instance.Move;
                if (inputVector != Vector2.zero)
                {
                    transform.position += new Vector3(inputVector.x, inputVector.y, 0f) * inputMoveSpeed * Time.unscaledDeltaTime;
                }
            }
        }

        UpdateRendererTint();
    }

    private void HandleActions()
    {
        // Confirm placement
        if (InputController.Instance.EnterPressed || InputController.Instance.MainWeaponPressed)
        {
            // If overlapping something on the Ally layer, do not allow placement
            if (IsOverlapping())
            {
                // play error sound and keep preview
                if (UIAudioSource != null && errorAudioClip != null)
                {
                    UIAudioSource.PlayOneShot(errorAudioClip);
                }

                return;
            }

            // Ensure player has enough coins
            if (inventory != null)
            {
                if (!inventory.HasCoins(price))
                {
                    if (UIAudioSource != null && errorAudioClip != null)
                    {
                        UIAudioSource.PlayOneShot(errorAudioClip);
                    }
                    return;
                }
                inventory.RemoveCoins(price);
            }

            upgradeController.ReopenUpgradePanel();
            Instantiate(prefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
            return;
        }

        // Cancel placement
        if (InputController.Instance.CancelPressed)
        {
            upgradeController.ReopenUpgradePanel();
            Destroy(gameObject);
            return;
        }
    }

    private bool IsOverlapping()
    {
        // Use Physics2D.OverlapPoint with the ally layer mask to determine if placement is valid
        if (allyLayerMask == 0) return false; // no ally layer configured
        Collider2D hit = Physics2D.OverlapPoint((Vector2)transform.position, allyLayerMask);
        return hit != null;
    }

    private void UpdateRendererTint()
    {
        spriteRenderer.color = IsOverlapping() ? new Color(1f, 0f, 0f, 1f) : new Color(150f/255f, 150f/255f, 150f/255f, 200f/255f);
    }
}
