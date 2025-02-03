using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [Header("Configuration")] [SerializeField]
    Transform weaponsParent;

    [Header("IK")] [SerializeField] Rig armsRig;
    [SerializeField] Rig aimRig;

    [Header("Inputs Combat")] [SerializeField]
    InputActionReference attackInputActionReference;

    [SerializeField] InputActionReference nextPrevWeapon;

    [Header("Imputs Fire Combat")] [SerializeField]
    InputActionReference shootInputActionReference;

    [SerializeField] InputActionReference continuousShootInputActionReference;
    [SerializeField] InputActionReference aimInputActionReference;
    [SerializeField] InputActionReference reloadInputActionReference;

    [Header("UI")] [SerializeField] Image weaponIcon;
    [SerializeField] Sprite[] weaponIcons;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI totalAmmoText;
    [SerializeField] private Image ammoBar;

    [Header("Audio")] [SerializeField] internal AudioSource audioSource;
    [SerializeField] AudioClip[] meleeAttackSounds;
    [SerializeField] AudioClip[] fireWeaponSounds;
    [SerializeField] internal AudioClip reloadSound;

    [Header("Events")] public UnityEvent onStartAiming;
    public UnityEvent onStopAiming;
    bool isAiming = false;

    Animator animator;
    RuntimeAnimatorController originalAnimatorController;

    internal WeaponBase[] weapons;
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
        reloadInputActionReference.action.Enable();
        //Add listeners
        attackInputActionReference.action.performed += OnAttack;
        nextPrevWeapon.action.performed += OnNextPrevWeapon;

        shootInputActionReference.action.performed += OnShoot;
        continuousShootInputActionReference.action.started += OnContinuousShoot;
        continuousShootInputActionReference.action.canceled += OnContinuousShoot;
        aimInputActionReference.action.started += OnAim;
         aimInputActionReference.action.canceled += OnAim;
        reloadInputActionReference.action.performed += OnReload;

        foreach (AnimationEventForwarder animationEventForwarder in GetComponentsInChildren<AnimationEventForwarder>())
        {
            animationEventForwarder.onMeleeAttackEvent.AddListener(OnMeleeAttackEvent);
            //if (weapons[currentWeaponIndex].audioSource != null) weapons[currentWeaponIndex].audioSource.Play();
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
        reloadInputActionReference.action.performed -= OnReload;

        shootInputActionReference.action.performed -= OnShoot;
        continuousShootInputActionReference.action.started -= OnContinuousShoot;
        continuousShootInputActionReference.action.canceled -= OnContinuousShoot;
        aimInputActionReference.action.started -= OnAim;
        aimInputActionReference.action.canceled -= OnAim;
        reloadInputActionReference.action.performed -= OnReload;

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
            if (weapons[currentWeaponIndex].audioSource != null && 
                currentWeaponIndex != 1 && 
                !(weapons[currentWeaponIndex] is Weapon_FireWeapon)) 
            {weapons[currentWeaponIndex].audioSource.Play();}
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
            Weapon_FireWeapon fireWeapon = (Weapon_FireWeapon)weapons[currentWeaponIndex];
            if (fireWeapon.currentAmmo > 0)
            {
                Debug.Log("Shooting");
                fireWeapon.Shoot();
                fireWeapon.audioSource.Play();
            }
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

    private void OnReload(InputAction.CallbackContext ctx)
    {
        
        if ((currentWeaponIndex != -1) && (weapons[currentWeaponIndex] is Weapon_FireWeapon))
        {
            ((Weapon_FireWeapon)weapons[currentWeaponIndex]).StartReloading();
            audioSource.PlayOneShot(reloadSound);
        }
    }

    private void OnAim(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        //animator.SetBool("IsAiming", value > 0f);

        animator.SetBool("IsAiming", value > 0f);
        PerformAim(value > 0f);
    }

    public void PerformAim(bool mustAim)
    {
        bool wasAiming = isAiming;
        isAiming = false;

        if (currentWeaponIndex != -1)
        {
            animator.SetBool("IsAiming", mustAim);
            isAiming = mustAim && weapons[currentWeaponIndex] is Weapon_FireWeapon;
        }

        //AnimateArmRigsWeight();
        //AnimateAimRigWeight();

        if (!wasAiming && isAiming)
        {
            onStartAiming.Invoke();
        }
        else if (wasAiming && !isAiming)
        {
            onStopAiming.Invoke();
        }
    }

    public void SelectWeapon(int weaponToSet)
    {
        // Deselect current weapon
        if (currentWeaponIndex != -1)
        {
            weapons[currentWeaponIndex].Deselect(animator);
            weapons[currentWeaponIndex].ContinueCooldown(this);
            if (weapons[currentWeaponIndex] is Weapon_FireWeapon)
            {
                animator.SetBool("IsShooting", false);
            }
        }

        // Select new weapon
        currentWeaponIndex = weaponToSet;
        if (currentWeaponIndex != -1)
        {
            weapons[currentWeaponIndex].gameObject.SetActive(true);
            weapons[currentWeaponIndex].Select(animator);
            animator.SetBool("IsHoldingFireWeapon", weapons[currentWeaponIndex] is Weapon_FireWeapon);
            weaponIcon.sprite = weaponIcons[currentWeaponIndex];
            weaponIcon.gameObject.SetActive(true);
        
            // Only set IsFighting if the weapon is not a fire weapon
            if (!(weapons[currentWeaponIndex] is Weapon_FireWeapon))
            {
                animator.SetBool("IsFighting", true);
            }

            SetFireWeaponAmmoText(weapons[currentWeaponIndex] as Weapon_FireWeapon);
        }
        else
        {
            animator.runtimeAnimatorController = originalAnimatorController;
            weaponIcon.sprite = null;
            weaponIcon.gameObject.SetActive(false);
            animator.SetBool("IsFighting", false);
            ammoBar.gameObject.SetActive(false);
        }

        AnimateArmRigsWeight();
        AnimateAimRigWeight();
    }

    public void UpdateAmmoText(Weapon_FireWeapon fireWeapon)
    {
        if (fireWeapon != null && GetComponent<PlayerController>() != null)
        {
            ammoText.text = fireWeapon.currentAmmo.ToString();
            totalAmmoText.text = fireWeapon.extraAmmo.ToString();
        }
    }

    private void SetFireWeaponAmmoText(Weapon_FireWeapon fireWeapon)
    {
        if (fireWeapon != null && GetComponent<PlayerController>() != null)
        {
            ammoBar.gameObject.SetActive(true);
            UpdateAmmoText(fireWeapon);
        }
        else
        {
            ammoBar.gameObject.SetActive(false);
        }
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
        Debug.Log("Melee attack event");
        PlayMeleeAttackSound();
        if (resetComboCoroutine != null)
        {
            StopCoroutine(resetComboCoroutine);
        }

        resetComboCoroutine = StartCoroutine(ResetComboAfterDelay());
        //weaponsParent.GetComponentInChildren<WeaponBase>().PerformAttack();
    }

    private void PlayMeleeAttackSound()
    {
        Debug.Log("Playing attack sound");
        WeaponBase currentWeapon = weapons[currentWeaponIndex];
        if (currentWeapon.audioSource != null && !(currentWeapon is Weapon_FireWeapon))
        {
            Debug.Log("Playing sound");
            currentWeapon.audioSource.Play();
        }
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

    public WeaponBase GetCurrentWeapon()
    {
        if (currentWeaponIndex != -1)
        {
            return weapons[currentWeaponIndex];
        }

        return null;
    }
}