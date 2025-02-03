using UnityEngine;

public abstract class BarrelBase : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool debugShoot;

    //Allows to call OnShoot() from the editor
    public void OnValidate()
    {
        if (debugShoot)
        {
            debugShoot = false;
            OnShoot();
        }
    }
    public abstract void OnShoot();
    public abstract void OnShootTarget(GameObject target);
}