using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : EnemyHealthBase
{
    [SerializeField] private string bossName = "Boss";
    private Image healthBarBorder;
    private Image healthBarFiller;
    private Slider healthBarSlider;
    private TextMeshProUGUI healthBarText;

    protected override void Start()
    {
        base.Start();

        healthBarBorder = GameObject.Find("BossHealthBar").GetComponent<Image>();
        healthBarFiller = GameObject.Find("HealthBarFiller").GetComponent<Image>();
        healthBarSlider = GameObject.Find("BossHealthBarSlider").GetComponent<Slider>();
        healthBarText = GameObject.Find("BossHealthbarNameLabel").GetComponent<TextMeshProUGUI>();

        healthBarBorder.enabled = true;
        healthBarFiller.enabled = true;
        healthBarText.text = bossName;
    }

    protected override void UpdateHealthBar()
    {
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = currentHealth;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        healthBarBorder.enabled = false;
        healthBarFiller.enabled = false;
        healthBarText.text = "";
    }
}
