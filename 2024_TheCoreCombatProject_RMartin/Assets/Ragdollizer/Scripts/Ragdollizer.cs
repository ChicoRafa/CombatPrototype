using System;
using UnityEngine;

public class Ragdollizer : MonoBehaviour
{
    Collider[] colliders;
    Rigidbody[] rigidbodies;

    #region Debug

    [SerializeField] bool debugRagdollize = false;

    private void OnValidate()
    {
        if (debugRagdollize)
        {
            debugRagdollize = false;
            Ragdollize();
        }
    }

    #endregion

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();

        UnRagdollize();
    }

    public void UnRagdollize()
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
    }

    public void Ragdollize()
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
        }
    }
}