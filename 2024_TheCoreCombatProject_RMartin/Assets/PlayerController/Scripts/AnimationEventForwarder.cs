using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class AnimationEventForwarder : MonoBehaviour
{
    [FormerlySerializedAs("onAnimationEvent")] [HideInInspector] public UnityEvent<string> onAnimationAttackEvent;
    [HideInInspector] public UnityEvent onMeleeAttackEvent;

    public void OnAnimationAttack(string hitColliderName)
    {
        Debug.Log($"OnAnimationAttack: {hitColliderName}");
        onAnimationAttackEvent.Invoke(hitColliderName);
    }

    public void MeleeWeaponAttack()
    {
        Debug.Log("MeleeWeaponAttack");
        onMeleeAttackEvent.Invoke();
    }
}