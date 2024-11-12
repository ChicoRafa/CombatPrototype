using UnityEngine;
using UnityEngine.Events;

public class AnimationEventForwarder : MonoBehaviour
{
    [HideInInspector] public UnityEvent<string> onAnimationEvent;

    public void OnAnimationAttack(string hitColliderName)
    {
        Debug.Log($"OnAnimationAttack: {hitColliderName}");
        onAnimationEvent.Invoke(hitColliderName);
    }
}