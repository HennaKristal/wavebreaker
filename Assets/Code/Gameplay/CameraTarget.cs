using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform HQTransform;
    private Vector3 HQPosition;

    [Header("Distances")]
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float gameOverDistance = 25f;

    [Header("Warnings")]
    [SerializeField] private GameObject warningLabel;
    [SerializeField] private GameObject warningLight;


    private void LateUpdate()
    {
        if (!player)
        {
            return;
        }

        TrackHQPosition();
        UpdateWarningSystem();
        FollowPlayerWithinBounds();
        UpdateWarningLightPosition();
    }

    private void TrackHQPosition()
    {
        if (HQTransform != null)
        {
            HQPosition = HQTransform.position;
        }
    }

    private void UpdateWarningSystem()
    {
        float playerToHQDistance = Vector2.Distance(player.position, HQPosition);

        // Fail if player goes too far from HQ
        if (playerToHQDistance > gameOverDistance)
        {
            GameManager.Instance.GameOver();
            this.enabled = false;
            return;
        }

        if (playerToHQDistance > maxDistance)
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

    private void FollowPlayerWithinBounds()
    {
        Vector3 desiredPosition = player.position;
        Vector2 offset = desiredPosition - HQPosition;
        float distance = offset.magnitude;

        if (distance > maxDistance)
        {
            desiredPosition = HQPosition + (Vector3)(offset.normalized * maxDistance);
        }

        transform.position = new Vector3(desiredPosition.x, desiredPosition.y, transform.position.z);
    }

    private void UpdateWarningLightPosition()
    {
        warningLight.transform.position = HQPosition;
    }
}
