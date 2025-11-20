using UnityEngine;

public class PreviewPlacement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject prefab;
    private ShopController shopController;
    private Inventory inventory;
    private Transform flagshipHQTransform;
    private SpriteRenderer spriteRenderer;
    private int allyLayerMask;
    private PolygonCollider2D polygonCollider;

    [Header("Movement")]
    [SerializeField] private float inputMoveSpeed = 3f;
    [SerializeField] private float mouseMoveThreshold = 0.01f;
    private Vector3 lastMousePosition;

    [Header("Price")]
    [SerializeField] private int price = 1000;

    [Header("Audio")]
    [SerializeField] private AudioClip errorAudioClip;
    private AudioSource UIAudioSource;


    private void Start()
    {
        inventory = GameManager.Instance.GetInventoryController();

        UIAudioSource = GameManager.Instance.GetUIAudioSource();

        flagshipHQTransform = GameManager.Instance.GetFlagshipHQTransform();
        transform.position = flagshipHQTransform.position;

        lastMousePosition = Input.mousePosition;

        shopController = GameObject.Find("ShopController")?.GetComponent<ShopController>();
        if (shopController == null)
        {
            Debug.LogError("PreviewPlacement: shopController not found in scene.");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        polygonCollider = GetComponent<PolygonCollider2D>();

        allyLayerMask = LayerMask.GetMask("Ally");
    }

    private void Update()
    {
        HandleMovement();
        HandleActions();
        UpdateRendererTint();
    }

    private void HandleMovement()
    {
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
            Vector2 inputVector = InputController.Instance.Move;
            if (inputVector != Vector2.zero)
            {
                transform.position += new Vector3(inputVector.x, inputVector.y, 0f) * inputMoveSpeed * Time.unscaledDeltaTime;
            }
        }
    }

    private void HandleActions()
    {
        // Confirm placement
        if (InputController.Instance.EnterPressed || InputController.Instance.MainWeaponPressed)
        {
            if (IsOverlapping() || !inventory.HasCoins(price))
            {
                UIAudioSource.PlayOneShot(errorAudioClip);
                return;
            }

            inventory.RemoveCoins(price);

            shopController.ReopenShopPanel();

            Instantiate(prefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
            return;
        }

        // Cancel placement
        if (InputController.Instance.CancelPressed)
        {
            shopController.ReopenShopPanel();
            Destroy(gameObject);
            return;
        }
    }

    private bool IsOverlapping()
    {
        Collider2D hit = Physics2D.OverlapPoint((Vector2)transform.position, allyLayerMask);
        return hit != null;
    }

    private void UpdateRendererTint()
    {
        spriteRenderer.color = IsOverlapping() ? new Color(1f, 0f, 0f, 1f) : new Color(150f/255f, 150f/255f, 150f/255f, 200f/255f);
    }
}
