using System;
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
    
    WeaponBase[] weapons;
    int currentWeaponIndex = -1;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        weapons = weaponsParent.GetComponentsInChildren<WeaponBase>();
        foreach (WeaponBase weaponBase in weapons)
        {
            weaponBase.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateCombat();
    }

    private void OnEnable()
    {
        attackInputActionReference.action.Enable();
        attackInputActionReference.action.performed += OnAttack;
        
        attackInputActionReference.action.performed += OnAttack;
        nextPrevWeapon.action.performed += OnNextPrevWeapon;
    }

    private void OnDisable()
    {
        attackInputActionReference.action.Disable();
        attackInputActionReference.action.performed -= OnAttack;
        
        attackInputActionReference.action.performed -= OnAttack;
        nextPrevWeapon.action.performed -= OnNextPrevWeapon;
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
            weapons[currentWeaponIndex].Deselect();
        }
        //Select new weapon
        currentWeaponIndex = weaponToSet;
        if (currentWeaponIndex != -1)
        {
            weapons[currentWeaponIndex].gameObject.SetActive(true);
            weapons[currentWeaponIndex].Select();
        }
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
