using UnityEngine;
using UnityEngine.Events;

public class HitCollider : MonoBehaviour, IHitter
{
    [Header("Configuration")] 
    public float damage;
    [Header("Events")]
    public UnityEvent OnHit; 
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<HurtCollider>(out HurtCollider hurtCollider))
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
