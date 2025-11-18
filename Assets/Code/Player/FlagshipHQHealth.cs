using UnityEngine;
using UnityEngine.UI;

public class FlagshipHQHealth : PlayerHealthBase
{
    [Header("UI")]
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Slider healthBarSlider;


    protected override void UpdateHealthUI()
    {
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Destroy(gameObject, explosionDuration + 1f);

            if (!GameManager.Instance.bossReached)
            {
                Invoke(nameof(GameOver), explosionDuration + 2f);
            }
        }
    }

    protected override void PlayTakeDamageSound()
    {

    }

    private void GameOver()
    {
        GameManager.Instance.GameOver();
    }
}
