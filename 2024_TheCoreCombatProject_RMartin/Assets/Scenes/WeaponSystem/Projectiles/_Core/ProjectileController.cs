using System;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    HitCollider hitCollider;
    private void Awake()
    {
        hitCollider = GetComponent<HitCollider>();
        
    }

    private void OnEnable()
    {
        hitCollider?.OnHit.AddListener(PerfomDestruction);
    }

    private void OnDisable()
    {
        hitCollider?.OnHit.RemoveListener(PerfomDestruction);
    }

    public void PerfomDestruction()
    {
        Destroy(gameObject);
    }
}
