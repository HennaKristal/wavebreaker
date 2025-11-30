using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform HQTransform;

    [Header("Distances")]
    [SerializeField] private float warningDistance = 20f;
    [SerializeField] private float gameOverDistance = 25f;

    [Header("Warnings")]
    [SerializeField] private GameObject warningLabel;
    [SerializeField] private GameObject warningLight;


    private void LateUpdate()
    {
        if (!player)
            return;

        UpdateWarningSystem();
        UpdateWarningLightPosition();
    }

    private void UpdateWarningSystem()
    {
        float playerToHQDistance = Vector2.Distance(player.position, HQTransform.position);

        // Fail if player goes too far from HQ
        if (playerToHQDistance > gameOverDistance)
        {
            GameManager.Instance.GameOver();
            this.enabled = false;
            return;
        }

        if (playerToHQDistance > warningDistance)
        {
            if (!warningLabel.activeSelf)
            {
                warningLabel.SetActive(true);
            }
        }
        else if (warningLabel.activeSelf)
        {
            warningLabel.SetActive(false);
        }
    }

    private void UpdateWarningLightPosition()
    {
        warningLight.transform.position = HQTransform.position;
    }
}
