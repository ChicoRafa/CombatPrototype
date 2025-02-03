using UnityEngine;

public class BarrelByInstantiation : BarrelBase
{
    [SerializeField] private GameObject projectile;
    public override void OnShoot()
    {
        Instantiate(projectile, transform.position, transform.rotation);
    }

    public override void OnShootTarget(GameObject target)
    {
        throw new System.NotImplementedException();
    }
}
