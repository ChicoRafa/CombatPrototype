using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EntityLife : MonoBehaviour
{
    [SerializeField] float initialLife = 1f;
    public UnityEvent<float> onLifeChanged;
    public UnityEvent onDeath;

    float currentLife;
    HurtCollider hurtCollider;
    Coroutine regenCoroutine;

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
        else
        {
            SetRegenerationCoroutineStatus();
        }
    }

    /*This method manages the health regeneration coroutine.*/
    private void SetRegenerationCoroutineStatus()
    {
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
        }

        if (GetComponent<PlayerController>() != null)
        {
            regenCoroutine = StartCoroutine(StartHealthRegen());
        }
    }

    private IEnumerator StartHealthRegen()
    {
        yield return new WaitForSeconds(10f);
        while (currentLife < initialLife)
        {
            currentLife += 5f;
            if (currentLife > initialLife)
            {
                currentLife = initialLife;
            }

            onLifeChanged.Invoke(currentLife);
            yield return new WaitForSeconds(1f);
        }
    }

    // This method is called when the player collides with a life power-up
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Life") && currentLife < initialLife)
        {
            PlayerController playerController = GetComponent<PlayerController>();
            if (playerController != null)
            {
                currentLife = initialLife;
                onLifeChanged.Invoke(currentLife);
                // Destroy the life power-up
                Destroy(other.gameObject);
            }
        }
    } 
    
    public float GetMaxLife()
    {
        return initialLife;
    }
}