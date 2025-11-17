using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayerController : PlayerHealthBase
{
    [Header("UI")]
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private AudioClip takeDamageSound;
    [SerializeField] private AudioSource audioSource;

    protected override void UpdateHealthUI()
    {
        healthBarSlider.maxValue = maxHealth;
        healthBarSlider.value = currentHealth;
        healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();

        if (currentHealth <= 0)
        {
            Invoke(nameof(GameOver), explosionDuration + 2f);
        }
    }

    protected override void PlayTakeDamageSound()
    {
        audioSource.PlayOneShot(takeDamageSound);
    }

    private void GameOver()
    {
        GameManager.Instance.LoadSceneByName("Game");
    }
}
