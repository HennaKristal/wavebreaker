using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : EnemyHealthBase
{
    [SerializeField] private string bossName = "Boss";
    private GameObject healthBar;
    private Slider healthBarSlider;
    private TextMeshProUGUI healthBarText;
    private bool referencedLoaded = false;


    private void LoadReferences()
    {
        healthBar = GameManager.Instance.GetBossHealthBar();
        healthBarSlider = GameManager.Instance.GetBossHealthBarSlider();
        healthBarText = GameManager.Instance.GetBossHealthBarText();
        referencedLoaded = true;
    }


    protected override void UpdateHealthBar()
    {
        if (!referencedLoaded)
        {
            LoadReferences();
        }

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
