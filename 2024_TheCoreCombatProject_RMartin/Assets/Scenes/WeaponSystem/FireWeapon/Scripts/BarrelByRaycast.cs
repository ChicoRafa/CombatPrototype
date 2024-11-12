using UnityEngine;

public class BarrelByRaycast : BarrelBase, IHitter
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float range = 50f;
    [SerializeField] private LayerMask layerMask = Physics.DefaultRaycastLayers;
    public override void OnShoot()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, range, layerMask))
        {
            hitInfo.collider.GetComponent<HurtCollider>()?.NotifyHit(this);
        }
    }

    float IHitter.GetDamage()
    {
        return damage;
    }
}

