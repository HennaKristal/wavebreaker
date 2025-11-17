using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : EnemyHealthBase
{
    [SerializeField] private string bossName = "Boss";
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TextMeshProUGUI healthBarText;

    protected override void UpdateHealthBar()
    {
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = currentHealth;

        if (currentHealth <= maxHealth && !isDead)
        {
            healthBar.SetActive(true);
            healthBarText.text = bossName;
        }

        if (currentHealth <= 0)
        {
            healthBar.SetActive(false);
        }
    }
}
