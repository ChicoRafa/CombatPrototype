using System;
using UnityEngine;
using UnityEngine.Events;

public class HitCollider : MonoBehaviour, IHitter
{
    [Header("Configuration")] public float damage;
    [Header("Events")] public UnityEvent OnHit;

    private void OnCollisionEnter(Collision collision)
    {
        CheckCollider(collision.collider);
    }

    private void OnTriggerEnter(Collider triggeredCollider)
    {
        CheckCollider(triggeredCollider);
    }

    private void CheckCollider(Collider otherCollider)
    {
        if (otherCollider.TryGetComponent<HurtCollider>(out HurtCollider hurtCollider))
        {
            hurtCollider.NotifyHit(this);
            OnHit.Invoke();
        }
    }

    float IHitter.GetDamage()
    {
        return damage;
    }
}