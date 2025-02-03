using System.Collections;
using UnityEngine;

public class Weapon_FireWeapon : WeaponBase
{
    BarrelBase[] barrels;

    [Header("Ammo Settings")]
    [SerializeField] internal int maxAmmo = 9; // Capacity of the magazine
    [SerializeField] internal int currentAmmo;
    [SerializeField] internal int extraAmmo = 45; // Extra ammo carried
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private bool isContinuousFire = false;
    public AmmoType ammoType;
    internal bool isReloading = false;
    private bool isShooting = false;
    
    private WeaponManager weaponManager;

    #region Debug
    // [Header("Debug")]
    // [SerializeField] private bool debugShoot = false;
    // [SerializeField] private bool debugStartShooting = false;
    // [SerializeField] private bool debugStopShooting = false;
    // [SerializeField] private bool debugShootBurst = false;
    // [SerializeField] private bool debugCancelBurst = false;
    //
    // private void OnValidate()
    // {
    //     if (debugShoot)
    //     {
    //         debugShoot = false;
    //         Shoot();
    //     }
    // }
    #endregion

    internal override void Init()
    {
        base.Init();
        barrels = GetComponentsInChildren<BarrelBase>();
        currentAmmo = maxAmmo;
        weaponManager = FindObjectOfType<WeaponManager>();
    }
    
    internal override void Select(Animator animator)
    {
        base.Select(animator);
        animator.SetBool("isHoldingFireWeapon", true);
        animator.SetBool("IsFighting", false);
        isReloading = false; //reset reloading state
    }

    internal override void Deselect(Animator animator)
    {
        base.Deselect(animator);
        animator.SetBool("isHoldingFireWeapon", false);
    }

    public void Shoot()
    {
        if (isReloading || isCooldownActive || currentAmmo <= 0)
        {
            return;
        }

        foreach (BarrelBase barrelBase in barrels)
        {
            barrelBase.OnShoot();
        }

        currentAmmo--;
        //si dispara player solo
        weaponManager.UpdateAmmoText(this);
        if (currentAmmo <= 0)
        {
            StartReloading();
        }
    }

    public void StartShooting()
    {
        if (isContinuousFire)
        {
            isShooting = true;
            StartCoroutine(ContinuousFire());
        }
        else
        {
            Shoot();
        }
    }

    public void StopShooting()
    {
        isShooting = false;
    }
    
    private IEnumerator ContinuousFire()
    {
        while (isShooting && currentAmmo > 0)
        {
            Shoot();
            yield return new WaitForSeconds(0.1f); // Adjust the fire rate as needed
        }
    }
    
    public void StartReloading()
    {
        if (!isReloading && extraAmmo > 0)
        {
            isReloading = true;
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, extraAmmo);

        currentAmmo += ammoToReload;
        extraAmmo -= ammoToReload;

        isReloading = false;
        weaponManager.UpdateAmmoText(this);
        audioSource.PlayOneShot(weaponManager.reloadSound);
    }
    
    public BarrelByRaycast GetBarrel()
{
    return GetComponentInChildren<BarrelByRaycast>();
}

    internal override void PerformAttack()
    {
        throw new System.NotImplementedException();
    }
    
    
    public void ShootToTarget(GameObject target)
    {
        // if (isReloading || currentAmmo <= 0)
        // {
        //     return;
        // }
        AimAtTarget(target);

        foreach (BarrelBase barrelBase in barrels)
        {
            barrelBase.OnShootTarget(target);
            audioSource.Play();
        }

       // currentAmmo--;
        //weaponManager.UpdateAmmoText(this);
        // if (currentAmmo <= 0)
        // {
        //     StartReloading();
        // }
    }

    private void AimAtTarget(GameObject target)
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

}