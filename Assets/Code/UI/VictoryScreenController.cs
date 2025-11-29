using TMPro;
using UnityEngine;

public class VictoryScreenController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI restartButton;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;
    private bool controlsEnabled = false;

    private void Start()
    {
        Invoke(nameof(EnableControls), 2f);
    }

    private void EnableControls()
    {
        controlsEnabled = true;
        restartButton.color = new Color(255f/255f, 2000f/255f, 0f/255f, 255f/255f);
    }

    private void Update()
    {
        if (controlsEnabled)
        {
            if (InputController.Instance.EnterPressed)
            {
                RestartGame();
            }
        }
    }

    public void RestartGame()
    {
        audioSource.PlayOneShot(clickSound);
        GameManager.Instance.GameOver();
    }
}
