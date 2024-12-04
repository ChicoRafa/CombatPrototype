using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("Configuration")] [SerializeField]
    Transform weaponsParent;
    
    [Header("IK")]
    [SerializeField] Rig armsRig;
    [SerializeField] Rig aimRig;

    [Header("Inputs Combat")] [SerializeField]
    InputActionReference attackInputActionReference;

    [SerializeField] InputActionReference nextPrevWeapon;

    [Header("Imputs Fire Combat")] [SerializeField]
    InputActionReference shootInputActionReference;

    [SerializeField] InputActionReference continuousShootInputActionReference;
    [SerializeField] InputActionReference aimInputActionReference;

    Animator animator;
    RuntimeAnimatorController originalAnimatorController;

    WeaponBase[] weapons;
    int currentWeaponIndex = -1;
    private Coroutine resetComboCoroutine;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        originalAnimatorController = animator.runtimeAnimatorController;
        weapons = weaponsParent.GetComponentsInChildren<WeaponBase>(true);
        foreach (WeaponBase weaponBase in weapons)
        {
            weaponBase.Init();
        }
    }

    private void Update()
    {
        UpdateCombat();
    }

    private void OnEnable()
    {
        //Enable all inputs
        attackInputActionReference.action.Enable();
        nextPrevWeapon.action.Enable();
        shootInputActionReference.action.Enable();
        continuousShootInputActionReference.action.Enable();
        aimInputActionReference.action.Enable();
        //Add listeners
        attackInputActionReference.action.performed += OnAttack;
        nextPrevWeapon.action.performed += OnNextPrevWeapon;

        shootInputActionReference.action.performed += OnShoot;
        continuousShootInputActionReference.action.started += OnContinuousShoot;
        continuousShootInputActionReference.action.canceled += OnContinuousShoot;
        aimInputActionReference.action.started += OnAim;
        aimInputActionReference.action.canceled += OnAim;

        foreach (AnimationEventForwarder animationEventForwarder in GetComponentsInChildren<AnimationEventForwarder>())
        {
            animationEventForwarder.onMeleeAttackEvent.AddListener(OnMeleeAttackEvent);
        }
    }

    private void OnDisable()
    {
        //Disable all inputs
        attackInputActionReference.action.Disable();
        nextPrevWeapon.action.Disable();
        shootInputActionReference.action.Disable();
        continuousShootInputActionReference.action.Disable();
        aimInputActionReference.action.Disable();
        //Remove all listeners
        attackInputActionReference.action.performed -= OnAttack;
        nextPrevWeapon.action.performed -= OnNextPrevWeapon;

        shootInputActionReference.action.performed -= OnShoot;
        continuousShootInputActionReference.action.started -= OnContinuousShoot;
        continuousShootInputActionReference.action.canceled -= OnContinuousShoot;
        aimInputActionReference.action.started -= OnAim;
        aimInputActionReference.action.canceled -= OnAim;

        foreach (AnimationEventForwarder animationEventForwarder in GetComponentsInChildren<AnimationEventForwarder>())
        {
            animationEventForwarder.onMeleeAttackEvent.RemoveListener(OnMeleeAttackEvent);
        }
    }

    bool mustAttack = false;

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!weapons[currentWeaponIndex].isCooldownActive)
        {
            mustAttack = true;
        }
    }

    private void OnNextPrevWeapon(InputAction.CallbackContext ctx)
    {
        int weaponToSetActive = currentWeaponIndex;
        Vector2 readValue = ctx.ReadValue<Vector2>();
        if (readValue.y > 0)
        {
            weaponToSetActive++;
            if (weaponToSetActive >= weapons.Length)
            {
                weaponToSetActive = -1;
            }
        }
        else
        {
            weaponToSetActive--;
            if (weaponToSetActive < -1)
            {
                weaponToSetActive = weapons.Length - 1;
            }
        }

        if (weaponToSetActive != currentWeaponIndex)
        {
            SelectWeapon(weaponToSetActive);
        }
    }

    private void OnShoot(InputAction.CallbackContext ctx)
    {
        if ((currentWeaponIndex != -1) && (weapons[currentWeaponIndex] is Weapon_FireWeapon))
        {
            ((Weapon_FireWeapon)weapons[currentWeaponIndex]).Shoot();
        }
    }

    private void OnContinuousShoot(InputAction.CallbackContext ctx)
    {
        if ((currentWeaponIndex != -1) && (weapons[currentWeaponIndex] is Weapon_FireWeapon))
        {
            float value = ctx.ReadValue<float>();
            if (value > 0f)
            {
                ((Weapon_FireWeapon)weapons[currentWeaponIndex]).StartShooting();
            }
            else
            {
                ((Weapon_FireWeapon)weapons[currentWeaponIndex]).StopShooting();
            }
        }
    }

    private void OnAim(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        animator.SetBool("IsAiming", value > 0f);
    }

    public void SelectWeapon(int weaponToSet)
    {
        //Deselect current weapon
        if (currentWeaponIndex != -1)
        {
            weapons[currentWeaponIndex].Deselect(animator);
            weapons[currentWeaponIndex].ContinueCooldown(this);
            if (weapons[currentWeaponIndex] is Weapon_FireWeapon)
            {
                animator.SetBool("IsShooting", false);
            }
        }

        //Select new weapon
        currentWeaponIndex = weaponToSet;
        if (currentWeaponIndex != -1)
        {
            weapons[currentWeaponIndex].gameObject.SetActive(true);
            weapons[currentWeaponIndex].Select(animator);
            animator.SetBool("IsHoldingFireWeapon", weapons[currentWeaponIndex] is Weapon_FireWeapon);
        }
        else
        {
            animator.runtimeAnimatorController = originalAnimatorController;
        }
        
        AnimateArmRigsWeight();
        AnimateAimRigWeight();
    }

    private void AnimateAimRigWeight()
    {
        DOTween.To(
            () => aimRig.weight,
            x => aimRig.weight = x,
            (currentWeaponIndex != -1) && (weapons[currentWeaponIndex] is Weapon_FireWeapon) ? 1f : 0f,
            0.25f
        );
    }

    private void AnimateArmRigsWeight()
    {
        DOTween.To(
            () => armsRig.weight,
            x => armsRig.weight = x,
            (currentWeaponIndex != -1) && (weapons[currentWeaponIndex] is Weapon_FireWeapon) ? 1f : 0f,
            0.25f
        );
    }

    public void OnMeleeAttackEvent()
    {
        weapons[currentWeaponIndex].PerformAttack();
        if (resetComboCoroutine != null)
        {
            StopCoroutine(resetComboCoroutine);
        }
        resetComboCoroutine = StartCoroutine(ResetComboAfterDelay());
        //weaponsParent.GetComponentInChildren<WeaponBase>().PerformAttack();
    }
    
    private IEnumerator ResetComboAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        weapons[currentWeaponIndex].ResetComboCount();
    }

    private void UpdateCombat()
    {
        if (mustAttack)
        {
            mustAttack = false;
            animator.SetTrigger("Attack");
        }
    }
    
    private Coroutine globalCooldownCoroutine;

    public void StartGlobalCooldownCoroutine(float remainingTime, WeaponBase weapon)
    {
        if (globalCooldownCoroutine != null)
        {
            StopCoroutine(globalCooldownCoroutine);
        }
        globalCooldownCoroutine = StartCoroutine(GlobalCooldownCoroutine(remainingTime, weapon));
    }

    private IEnumerator GlobalCooldownCoroutine(float remainingTime, WeaponBase weapon)
    {
        yield return new WaitForSeconds(remainingTime);
        weapon.isCooldownActive = false;
        weapon.ResetComboCount();
    }
}