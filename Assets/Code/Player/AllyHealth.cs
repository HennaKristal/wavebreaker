using UnityEngine;
using UnityEngine.UI;

public class AllyHealth : PlayerHealthBase
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
            healthBar.SetActive(false);
            Destroy(gameObject, explosionDuration + 1f);
        }
        else if (!healthBar.activeSelf)
        {
            healthBar.SetActive(true);
        }
    }

    protected override void PlayTakeDamageSound()
    {

    }
}
