using UnityEngine;

public class VictoryScreenController : MonoBehaviour
{
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;
    private bool controlsEnabled = false;

    public void ShowEndingScreen()
    {
        victoryScreen.SetActive(true);
        Invoke(nameof(EnableControls), 2f);
    }

    private void EnableControls()
    {
        controlsEnabled = true;
    }

    private void Update()
    {
        if (controlsEnabled)
        {
            if (InputManager.Instance.EnterPressed)
            {
                RestartGame();
            }
        }
    }

    private void RestartGame()
    {
        audioSource.PlayOneShot(clickSound);
        GameManager.Instance.GameOver();
    }
}
