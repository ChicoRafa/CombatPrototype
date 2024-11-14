using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] private int comboLenght = 3;
    [SerializeField] AnimatorOverrideController animatorOverrideController;
    [SerializeField] private bool activateFighting = false;
    internal virtual void Init()
     {
         gameObject.SetActive(false);
     }
    internal virtual void Select(Animator animator)
     {
            gameObject.SetActive(true);
            animator.runtimeAnimatorController = animatorOverrideController;
            animator.SetBool("IsFighting", activateFighting);
            animator.SetInteger("ComboLength", comboLenght);
     }
    internal virtual void Deselect(Animator animator)
    {
        gameObject.SetActive(false);
        animator.runtimeAnimatorController = null;
    }

    internal abstract void PerformAttack();
}
