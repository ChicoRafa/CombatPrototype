using UnityEngine;

public class WeaponMelee_ByHitCollider : WeaponBase
{
    [SerializeField] Transform hitCollider;
    internal override void Deselect(Animator animator)
    {
        base.Deselect(animator);
        hitCollider?.gameObject.SetActive(false);
    }
    internal override void PerformAttack()
    {
        if (!isCooldownActive)
        {
            hitCollider?.gameObject.SetActive(true);
            IncrementComboCount();
        }
    }
}
