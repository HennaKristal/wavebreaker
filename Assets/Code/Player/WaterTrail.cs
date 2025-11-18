using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WaterTrail : MonoBehaviour
{
    private class TrailPoint
    {
        public Vector3 position;
        public float timeAlive;

        public TrailPoint(Vector3 pos)
        {
            position = pos;
            timeAlive = 0f;
        }
    }

    [Header("Trail Settings")]
    [SerializeField] private int maxLength = 50;
    [SerializeField] private float minDistance = 0.1f;
    [SerializeField] private float pointLifetime = 2f;
    [SerializeField] private float minWidth = 0.35f;
    [SerializeField] private float maxWidth = 1.0f;
    [SerializeField] private SpriteRenderer waterRippleRenderer;

    private Queue<TrailPoint> pointQueue = new Queue<TrailPoint>();
    private LineRenderer lineRenderer;
    private Vector3 lastPos;


    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;

        lastPos = transform.position;
        pointQueue.Enqueue(new TrailPoint(lastPos));
    }

    private void FixedUpdate()
    {
        Vector3 currentPos = transform.position;

        if (Vector3.Distance(currentPos, lastPos) >= minDistance)
        {
            pointQueue.Enqueue(new TrailPoint(currentPos));
            lastPos = currentPos;
        }

        // Update lifetime
        foreach (var point in pointQueue)
        {
            point.timeAlive += Time.deltaTime;
        }

        // Remove old points
        while (pointQueue.Count > 0 && pointQueue.Peek().timeAlive > pointLifetime)
        {
            pointQueue.Dequeue();
        }
        while (pointQueue.Count > maxLength)
        {
            pointQueue.Dequeue();
        }

        // Display Water ripple if no trail points
        if (waterRippleRenderer != null)
        {
            if (pointQueue.Count == 0)
            {
                waterRippleRenderer.enabled = true;
                return;
            }
            else if (waterRippleRenderer.enabled)
            {
                waterRippleRenderer.enabled = false;
            }
        }

        // Display water trail
        var array = System.Array.ConvertAll(pointQueue.ToArray(), p => p.position);
        lineRenderer.positionCount = array.Length;
        lineRenderer.SetPositions(array);

        AnimationCurve widthCurve = new AnimationCurve();
        float growthFactor = Mathf.Clamp01((float)array.Length / maxLength);
        float currentBoatWidth = Mathf.Lerp(minWidth, maxWidth, growthFactor);

        widthCurve.AddKey(0f, currentBoatWidth);
        widthCurve.AddKey(1f, minWidth);

        lineRenderer.widthCurve = widthCurve;
    }
}
