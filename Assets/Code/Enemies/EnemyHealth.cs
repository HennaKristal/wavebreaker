using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : EnemyHealthBase
{
    [Header("UI")]
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Slider healthBarSlider;


    protected override void UpdateHealthBar()
    {
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            healthBar.SetActive(false);
        }
        else if (!healthBar.activeSelf)
        {
            healthBar.SetActive(true);
        }
    }
}
