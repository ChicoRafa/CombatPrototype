using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class AnimationEventForwarder : MonoBehaviour
{
    [FormerlySerializedAs("onAnimationEvent")] [HideInInspector] public UnityEvent<string> onAnimationAttackEvent;
    [HideInInspector] public UnityEvent onMeleeAttackEvent;

    public void OnAnimationAttack(string hitColliderName)
    {
        onAnimationAttackEvent.Invoke(hitColliderName);
    }

    public void MeleeWeaponAttack()
    {
        onMeleeAttackEvent.Invoke();
    }
}