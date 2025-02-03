using UnityEngine;

public class WeaponMelee_MartialArts : WeaponBase
{
    private void OnEnable()
    {
        foreach (AnimationEventForwarder animationEventForwarder in GetComponentsInChildren<AnimationEventForwarder>())
        {
            animationEventForwarder.onMeleeAttackEvent.AddListener(OnMeleeAttackEvent);
        }
    }

    private void OnDisable()
    {
        foreach (AnimationEventForwarder animationEventForwarder in GetComponentsInChildren<AnimationEventForwarder>())
        {
            animationEventForwarder.onMeleeAttackEvent.RemoveListener(OnMeleeAttackEvent);
        }
    }

    private void OnMeleeAttackEvent()
    {
        // Implement the attack logic here
        Debug.Log("Melee attack event received");
    }

    internal override void PerformAttack()
    {
        throw new System.NotImplementedException();
    }
}