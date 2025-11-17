using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraTarget : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform HQTransform;

    [Header("Distances")]
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float killDistance = 25f;

    [Header("Warning Light")]
    [SerializeField] private GameObject warningLabel;
    [SerializeField] private GameObject warningLight;

    private Vector3 HQLastPosition;
    private bool HQDestroyed = false;


    private void LateUpdate()
    {
        if (!player) return;

        TrackHQPosition();
        Vector3 hqPos = HQDestroyed ? HQLastPosition : HQTransform.position;

        UpdateWarningSystem(hqPos);
        FollowPlayerWithinBounds(hqPos);
        UpdateWarningLightPosition(hqPos);
    }

    private void TrackHQPosition()
    {
        if (!HQDestroyed)
        {
            if (HQTransform == null)
            {
                HQDestroyed = true;
            }
            else
            {
                HQLastPosition = HQTransform.position;
            }
        }
    }

    private void UpdateWarningSystem(Vector3 HQPosition)
    {
        float distance = Vector2.Distance(player.position, HQPosition);

        // Kill if too far
        if (distance > killDistance)
        {
            GameManager.Instance.GameOver();
            this.enabled = false;
            return;
        }

        warningLabel.SetActive(distance > maxDistance);
    }

    private void FollowPlayerWithinBounds(Vector3 HQPosition)
    {
        Vector3 desiredPos = player.position;

        Vector2 offset = desiredPos - HQPosition;
        float distance = offset.magnitude;

        if (distance > maxDistance)
        {
            desiredPos = HQPosition + (Vector3)(offset.normalized * maxDistance);
        }

        transform.position = new Vector3(desiredPos.x, desiredPos.y, transform.position.z);
    }

    private void UpdateWarningLightPosition(Vector3 HQPosition)
    {
        if (warningLight)
        {
            warningLight.transform.position = HQPosition;
        }
    }
}
