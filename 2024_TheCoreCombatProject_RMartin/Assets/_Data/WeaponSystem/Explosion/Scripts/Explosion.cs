using UnityEngine;

public class Explosion : MonoBehaviour, IHitter
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float radius = 10f;
    [SerializeField] private float explosionForce = 200f;
    [SerializeField] private LayerMask targetLayerMask = Physics.DefaultRaycastLayers;
    [SerializeField] private LayerMask occluderLayerMask = Physics.DefaultRaycastLayers;
    [SerializeField] private GameObject visualExplosionPrefab;

    void Start()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, radius, targetLayerMask))
        {
            if (!Physics.Linecast(transform.position, collider.transform.position, out RaycastHit hit, occluderLayerMask)
                || hit.collider == collider)
            {
                Debug.Log("Collider was hit: " + collider.name);
                // Collider was hit
                collider.GetComponent<HurtCollider>()?.NotifyHit(this);
            }

            collider.attachedRigidbody?.AddExplosionForce(explosionForce, transform.position, radius);
        }

        GameObject visualExplosionInstance = Instantiate(visualExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(visualExplosionInstance, 3f); // Destroy the instantiated object after 3 seconds
    }

    float IHitter.GetDamage()
    {
        return damage;
    }
}