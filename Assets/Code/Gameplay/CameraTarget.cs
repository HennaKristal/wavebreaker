using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] private Vector2 xLimit;
    [SerializeField] private Vector2 yLimit;
    [SerializeField] private Transform player;


    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = transform.position;

        targetPosition.x = Mathf.Clamp(player.position.x, xLimit.x, xLimit.y);
        targetPosition.y = Mathf.Clamp(player.position.y, yLimit.x, yLimit.y);

        transform.position = targetPosition;
    }
}
