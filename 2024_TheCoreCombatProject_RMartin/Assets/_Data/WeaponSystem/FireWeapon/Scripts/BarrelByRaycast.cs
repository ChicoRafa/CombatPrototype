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
        bool hitDetected = false;

        // Forward raycast
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, range, layerMask))
        {
            hitInfo.collider.GetComponent<HurtCollider>()?.NotifyHit(this);
            bulletEndPosition = hitInfo.point;
            hitDetected = true;
        }

        // Backward raycast
        if (!hitDetected && Physics.Raycast(transform.position, -transform.forward, out hitInfo, range, layerMask))
        {
            hitInfo.collider.GetComponent<HurtCollider>()?.NotifyHit(this);
            bulletEndPosition = hitInfo.point;
        }

        Instantiate(bulletTrailPrefab)?.GetComponent<BulletTrail>()?.InitBullet(bulletStartPosition, bulletEndPosition);
    }

    private void ShootShotgun()
    {
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = GetSpreadDirection();
            Vector3 bulletStartPosition = transform.position;
            Vector3 bulletEndPosition = transform.position + direction * range;
            bool hitDetected = false;

            // Forward raycast
            if (Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, range, layerMask))
            {
                hitInfo.collider.GetComponent<HurtCollider>()?.NotifyHit(this);
                bulletEndPosition = hitInfo.point;
                hitDetected = true;
            }

            // Backward raycast
            if (!hitDetected && Physics.Raycast(transform.position, -direction, out hitInfo, range, layerMask))
            {
                hitInfo.collider.GetComponent<HurtCollider>()?.NotifyHit(this);
                bulletEndPosition = hitInfo.point;
            }

            Instantiate(bulletTrailPrefab)?.GetComponent<BulletTrail>()
                ?.InitBullet(bulletStartPosition, bulletEndPosition);
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

    public float GetShootRange()
    {
        return range;
    }
    
    public override void OnShootTarget(GameObject target)
    {
        if (target == null) return;
        if (isShotgun)
        {
            ShootShotgunWithTarget(target);
        }
        else
        {
            ShootPistolWithTarget(target);
        }
    }

    private void ShootShotgunWithTarget(GameObject target)
    { 
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = GetSpreadDirectionWithTarget(target);
            Vector3 bulletStartPosition = transform.position;
            Vector3 bulletEndPosition = transform.position + direction * range;

            if (Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, range, layerMask))
            {
                hitInfo.collider.GetComponent<HurtCollider>()?.NotifyHit(this);
                bulletEndPosition = hitInfo.point;
            }

            Instantiate(bulletTrailPrefab)?.GetComponent<BulletTrail>()?.InitBullet(bulletStartPosition, bulletEndPosition);
        }
    }

    private Vector3 GetSpreadDirectionWithTarget(GameObject target)
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        direction.y += 0.5f;
        float angle = Random.Range(-spreadAngle, spreadAngle);
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        return rotation * direction;
    }

    private void ShootPistolWithTarget(GameObject target)
    {
        Vector3 bulletStartPosition = transform.position;
        Vector3 direction = (target.transform.position - transform.position).normalized;
        direction.y += 0.5f;
        Vector3 bulletEndPosition = transform.position + direction * range;
        
        if (Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, range, layerMask))
        {
            hitInfo.collider.GetComponent<HurtCollider>()?.NotifyHit(this);
            bulletEndPosition = hitInfo.point;
        }

        Instantiate(bulletTrailPrefab)?.GetComponent<BulletTrail>()?.InitBullet(bulletStartPosition, bulletEndPosition);
    }
}