using System;
using DG.Tweening;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float startSpeed = 10f;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float timeToDieAfterCollision = 0f;
    [SerializeField] private float lifeTime = 10f;
    HitCollider hitCollider;

    private void Awake()
    {
        hitCollider = GetComponent<HitCollider>();
        DOVirtual.DelayedCall(lifeTime, () => Destroy(gameObject));
    }

    private void Start()
    {
        GetComponent<Rigidbody>().linearVelocity = transform.forward * startSpeed;
            
    }

    private void OnEnable()
    {
        hitCollider?.OnHit.AddListener(PerfomDestruction);
    }

    private Tween deathTween = null;

    private void OnCollisionEnter(Collision collision)
    {
        if (deathTween == null)
        {
            deathTween = DOVirtual.DelayedCall(
                timeToDieAfterCollision,
                () =>
                {
                    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                });
        }
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