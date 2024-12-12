using System;
using DG.Tweening;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    [SerializeField] float trailDuration = 0.25f;
    LineRenderer lineRenderer;
    const int MAX_POSITIONS = 10;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void InitBullet(Vector3 startPosition, Vector3 endPosition)
    {
        Vector3[] positions = new Vector3[MAX_POSITIONS];
        for (int i = 0; i < MAX_POSITIONS; i++)
        {
            float interpolationValue  =  i / (float)MAX_POSITIONS;
            positions[i] = Vector3.Lerp(startPosition, endPosition, interpolationValue);
        }
        lineRenderer.SetPositions(positions);
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.yellow;
        
        DOTween.To(
            () => lineRenderer.widthMultiplier,
            (x) => lineRenderer.widthMultiplier = x,
            0f,
            trailDuration
        ).OnComplete(() => Destroy(gameObject));
    }
}
