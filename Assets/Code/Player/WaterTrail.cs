using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WaterTrail : MonoBehaviour
{
    [Header("Trail Settings")]
    public int maxLength = 50;
    public float minDistance = 0.1f;
    public float pointLifetime = 2f;

    [Header("Width Settings")]
    public float minWidth = 0.35f;
    public float maxWidth = 1.0f;

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

    private Queue<TrailPoint> points = new Queue<TrailPoint>();
    private LineRenderer lineRenderer;
    private Vector3 lastPos;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;

        lastPos = transform.position;
        points.Enqueue(new TrailPoint(lastPos));
    }

    void FixedUpdate()
    {
        Vector3 currentPos = transform.position;

        if (Vector3.Distance(currentPos, lastPos) >= minDistance)
        {
            points.Enqueue(new TrailPoint(currentPos));
            lastPos = currentPos;
        }

        // Update lifetime
        foreach (var p in points)
            p.timeAlive += Time.deltaTime;

        // Remove old points
        while (points.Count > 0 && points.Peek().timeAlive > pointLifetime)
            points.Dequeue();

        while (points.Count > maxLength)
            points.Dequeue();

        if (points.Count == 0)
            return;

        var array = System.Array.ConvertAll(points.ToArray(), p => p.position);
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
