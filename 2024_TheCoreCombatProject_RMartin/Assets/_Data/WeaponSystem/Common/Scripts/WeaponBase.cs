using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] private int comboLenght = 3;
    [SerializeField] private float comboCooldown = 3f;
    [SerializeField] AnimatorOverrideController animatorOverrideController;
    [SerializeField] private bool activateFighting = false;
    [SerializeField] protected internal bool isCooldownActive = false;
    [SerializeField] protected int currentComboCount = 0;
    private float cooldownStartTime = 0f;
    public AudioSource audioSource;

    internal virtual void Init()
     {
         gameObject.SetActive(false);
         audioSource = GetComponent<AudioSource>();
         Animator animator = GetComponent<Animator>();
         if (animator != null)
         {
             animator.SetBool("IsFighting", false);
         }
     }
    internal virtual void Select(Animator animator)
     {
            gameObject.SetActive(true);
            animator.runtimeAnimatorController = animatorOverrideController;
            if (!(this is Weapon_FireWeapon))
            {
                animator.SetBool("IsFighting", activateFighting);
            }
            animator.SetInteger("ComboLength", comboLenght);
     }
    internal virtual void Deselect(Animator animator)
    {
        gameObject.SetActive(false);
        cooldownStartTime = Time.time;
        animator.runtimeAnimatorController = null;
        ResetComboCount();
    }

    internal abstract void PerformAttack();
    
    protected IEnumerator ComboCooldownCoroutine()
    {
        isCooldownActive = true;
        yield return new WaitForSeconds(comboCooldown);
        isCooldownActive = false;
    }

    protected void StartComboCooldown()
    {
        if (!isCooldownActive)
        {
            StartCoroutine(ComboCooldownCoroutine());
        }
    }
    
    internal void ContinueCooldown(WeaponManager weaponManager)
    {
        if (isCooldownActive)
        {
            weaponManager.StartGlobalCooldownCoroutine(comboCooldown - (Time.time - cooldownStartTime), this);
        }
    }
    
    protected void IncrementComboCount()
    {
        currentComboCount++;
        if (currentComboCount >= comboLenght)
        {
            StartComboCooldown();
            currentComboCount = 0;
        }
    }
    
    protected internal void ResetComboCount()
    {
        currentComboCount = 0;
    }
}
