using UnityEngine;
using UnityEngine.InputSystem;
public class WeaponManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] Transform weaponsParent;
    
    [Header("Inputs Combat")]
    [SerializeField] InputActionReference attackInputActionReference;
    [SerializeField] InputActionReference nextPrevWeapon;
    
    Animator animator;
    RuntimeAnimatorController originalAnimatorController;
    
    WeaponBase[] weapons;
    int currentWeaponIndex = -1;

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
        attackInputActionReference.action.Enable();        
        nextPrevWeapon.action.Enable();

        attackInputActionReference.action.performed += OnAttack;
        nextPrevWeapon.action.performed += OnNextPrevWeapon;
        
        foreach (AnimationEventForwarder animationEventForwarder in GetComponentsInChildren<AnimationEventForwarder>())
        {
            
            animationEventForwarder.onAnimationAttackEvent.AddListener(OnMeleeAttackEvent);
        }
    }

    private void OnDisable()
    {
        attackInputActionReference.action.Disable();
        nextPrevWeapon.action.Disable();
        
        attackInputActionReference.action.performed -= OnAttack;
        nextPrevWeapon.action.performed -= OnNextPrevWeapon;
        
        foreach (AnimationEventForwarder animationEventForwarder in GetComponentsInChildren<AnimationEventForwarder>())
        {
            animationEventForwarder.onAnimationAttackEvent.RemoveListener(OnMeleeAttackEvent);
        }
    }
    
    bool mustAttack = false;
    private void OnAttack(InputAction.CallbackContext ctx)
    {
        mustAttack = true;
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

    public void SelectWeapon(int weaponToSet)
    {
        //Deselect current weapon
        if (currentWeaponIndex != -1)
        {
            weapons[currentWeaponIndex].Deselect(animator);
        }
        //Select new weapon
        currentWeaponIndex = weaponToSet;
        if (currentWeaponIndex != -1)
        {
            weapons[currentWeaponIndex].gameObject.SetActive(true);
            weapons[currentWeaponIndex].Select(animator);
        }
        else
        {
            animator.runtimeAnimatorController = originalAnimatorController;
        }
    }
    
    public void OnMeleeAttackEvent(string arg0)
    {
        weapons[currentWeaponIndex].PerformAttack();
        //weaponsParent.GetComponentInChildren<WeaponBase>().PerformAttack();

    }
    
    private void UpdateCombat()
    {
        if (mustAttack)
        {
            mustAttack = false;
            animator.SetTrigger("Attack");
        }
    }
}
