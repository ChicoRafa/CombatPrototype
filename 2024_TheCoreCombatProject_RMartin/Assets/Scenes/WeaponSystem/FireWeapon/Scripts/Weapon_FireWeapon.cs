using System;
using UnityEngine;

public class Weapon_FireWeapon : WeaponBase
{
    BarrelBase[] barrels;
    #region Debug
    [Header("Debug")]
    [SerializeField] private bool debugShoot = false;
    
    [SerializeField] private bool debugStartShooting = false;
    [SerializeField] private bool debugStopShooting = false;
    
    [SerializeField] private bool debugShootBurst = false;
    [SerializeField] private bool debugCancelBurst = false;
    

    private void OnValidate()
    {
        if (debugShoot)
        {
            debugShoot = false;
            Shoot();
        }
    }
    #endregion

    internal override void Init()
    {
        base.Init();
        barrels = GetComponentsInChildren<BarrelBase>();
    }

    public void Shoot()
    {
        foreach (BarrelBase barrelBase in barrels)
        {
            barrelBase.OnShoot();
        }
    }
    
    public void StartShooting(){}
    
    public void StopShooting(){}

    internal override void PerformAttack()
    {
        throw new System.NotImplementedException();
    }
}