using UnityEngine;

public class Explosion : MonoBehaviour, IHitter
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float radius = 10f;
    [SerializeField] private float explosionForce = 200f;
    [SerializeField] private LayerMask targetLayerMask = Physics.DefaultRaycastLayers;
    [SerializeField] private LayerMask occluderLayerMask = Physics.DefaultRaycastLayers;

    void Start()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, radius, targetLayerMask))
        {
            if (!Physics.Linecast(transform.position, collider.transform.position, out RaycastHit hit,
                    occluderLayerMask)
                || hit.collider == collider)
            {
                //Collider was hit
                hit.collider?.GetComponent<HurtCollider>()?.NotifyHit(this);
            }

            hit.collider?.attachedRigidbody?.AddExplosionForce(explosionForce, transform.position, radius);
        }
    }

    float IHitter.GetDamage()
    {
        return damage;
    }
}