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
        Debug.Log("PerformAttack");
        hitCollider?.gameObject.SetActive(true);
    }
}
