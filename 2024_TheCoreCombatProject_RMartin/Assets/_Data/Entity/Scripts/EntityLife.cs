using System;
using UnityEngine;
using UnityEngine.Events;

public class EntityLife : MonoBehaviour
{
    [SerializeField] float initialLife = 1f;
    public UnityEvent <float> onLifeChanged;
    public UnityEvent onDeath;
    
    float currentLife;
    HurtCollider hurtCollider;
    
    #region Debug
    [SerializeField] float debugLifeToAdd = 0f;
    [SerializeField] float debugLifeToSubtract = 0f;
    [SerializeField] bool debugApplyLifeChange;

    private void OnValidate()
    {
        if (debugApplyLifeChange)
        {
            debugApplyLifeChange = false;
            OnHitWithDamage(debugLifeToSubtract);
        }
    }
    #endregion

    private void Awake()
    {
        hurtCollider = GetComponentInChildren<HurtCollider>();
        currentLife = initialLife;
    }

    private void OnEnable()
    {
        hurtCollider.OnHitWithDamage.AddListener(OnHitWithDamage);

    }
    
    private void OnDisable()
    {
        hurtCollider.OnHitWithDamage.RemoveListener(OnHitWithDamage);
    }

    private void OnHitWithDamage(float damage)
    {
        currentLife -= damage;
        onLifeChanged.Invoke(currentLife);
        if (currentLife <= 0f)
        {
            onDeath.Invoke();
        }
    }
}
