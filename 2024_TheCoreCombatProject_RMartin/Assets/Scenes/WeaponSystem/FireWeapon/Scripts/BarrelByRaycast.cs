using UnityEngine;

public class BarrelByRaycast : BarrelBase, IHitter
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float range = 50f;
    [SerializeField] private LayerMask layerMask = Physics.DefaultRaycastLayers;
    [SerializeField] private GameObject bulletTrailPrefab;
    public override void OnShoot()
    {
        Vector3 bulletStartPosition = transform.position;
        Vector3 bulletEndPosition = transform.position + transform.forward * range;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, range, layerMask))
        {
            hitInfo.collider.GetComponent<HurtCollider>()?.NotifyHit(this);
            bulletEndPosition = hitInfo.point;
        }
        Instantiate(bulletTrailPrefab)?.
            GetComponent<BulletTrail>()?.
            InitBullet(bulletStartPosition, bulletEndPosition);
    }

    float IHitter.GetDamage()
    {
        return damage;
    }
}

