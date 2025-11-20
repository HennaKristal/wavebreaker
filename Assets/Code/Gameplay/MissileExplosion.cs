using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

public class MissileExplosion : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int damage = 1000;
    public float duration = 5f;
    [SerializeField] private float fullDamageTimeWindow = 0.1f;
    private float startTime;

    [Header("Shake")]
    [SerializeField] private Settings settings;
    [SerializeField] private ShakeData explosionShake;


    private void Start()
    {
        startTime = Time.time;

        if (settings.screenShakeEnabled)
        {
            Debug.Log("shake");
            CameraShakerHandler.Shake(explosionShake);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ally"))
        {
            var playerController = other.gameObject.GetComponent<PlayerHealthBase>();
            float elapsedTime = Time.time - startTime;
            float damageModifier;

            if (elapsedTime <= fullDamageTimeWindow)
            {
                damageModifier = 1f;
            }
            else if (elapsedTime <= duration)
            {
                float t = (elapsedTime - fullDamageTimeWindow) / (duration - fullDamageTimeWindow);
                damageModifier = Mathf.Lerp(0.5f, 0f, t);
            }
            else
            {
                damageModifier = 0f;
            }

            int finalDamage = Mathf.RoundToInt(damage * damageModifier);
            if (finalDamage > 0)
            {
                playerController?.TakeDamage(finalDamage);
            }
        }
    }
}
