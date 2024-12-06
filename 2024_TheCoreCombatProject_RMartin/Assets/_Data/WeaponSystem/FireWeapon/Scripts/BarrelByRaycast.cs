using UnityEngine;

public class BarrelByRaycast : BarrelBase, IHitter
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float range = 50f;
    [SerializeField] private LayerMask layerMask = Physics.DefaultRaycastLayers;
    [SerializeField] private GameObject bulletTrailPrefab;
    [SerializeField] private bool isShotgun = false;
    [SerializeField] private int pelletCount = 10;
    [SerializeField] private float spreadAngle = 10f;

    public override void OnShoot()
    {
        if (isShotgun)
        {
            ShootShotgun();
        }
        else
        {
            ShootPistol();
        }
    }

    private void ShootPistol()
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

    private void ShootShotgun()
    {
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = GetSpreadDirection();
            Vector3 bulletStartPosition = transform.position;
            Vector3 bulletEndPosition = transform.position + direction * range;

            if (Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, range, layerMask))
            {
                hitInfo.collider.GetComponent<HurtCollider>()?.NotifyHit(this);
                bulletEndPosition = hitInfo.point;
            }

            Instantiate(bulletTrailPrefab)?.
                GetComponent<BulletTrail>()?.
                InitBullet(bulletStartPosition, bulletEndPosition);
        }
    }

    private Vector3 GetSpreadDirection()
    {
        float angle = Random.Range(-spreadAngle, spreadAngle);
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        return rotation * transform.forward;
    }

    float IHitter.GetDamage()
    {
        return damage;
    }
}