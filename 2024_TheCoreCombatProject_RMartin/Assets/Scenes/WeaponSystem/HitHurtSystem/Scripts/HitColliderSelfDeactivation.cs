using System;
using DG.Tweening;
using UnityEngine;

public class HitColliderSelfDeactivation : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    
    Tween selfDeactivationTween;
    private void OnEnable()
    {
        selfDeactivationTween = DOVirtual.DelayedCall(duration, () => gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        if (selfDeactivationTween != null)
        {
            selfDeactivationTween.Kill();
            selfDeactivationTween = null;
        }
    }
}
