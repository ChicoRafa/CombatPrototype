using UnityEngine;

public class WeaponMelee_ByRaycast : WeaponBase, IHitter
{
    [SerializeField] float damage = 1f;
    [SerializeField] Transform raycastStart;
    [SerializeField] Transform raycastEnd;
    
    [SerializeField] LayerMask layerMask = Physics.DefaultRaycastLayers;
    
    [SerializeField] float minimumPointDistance = 0.05f;
    [SerializeField] float attackDuration = 0.5f;
    
    float remainingAttackDuration = 0f;
    private Vector3 oldRaycastStart;
    private Vector3 oldRaycastEnd;
    private void Update()
    {
        remainingAttackDuration -= Time.deltaTime;
        if (remainingAttackDuration <= 0f)
        {
            float distanceBetweenStarts = (oldRaycastStart - raycastStart.position).magnitude;
            float distanceBetweenEnds = (oldRaycastEnd - raycastEnd.position).magnitude;
            int raysToUse = Mathf.CeilToInt(Mathf.Max(distanceBetweenStarts, distanceBetweenEnds) / minimumPointDistance);

            for (int i = 0; i < raysToUse; i++)
            {
                float lerpValue = (float)i / (float)raysToUse;
                /*Debug.DrawLine(
                    Vector3.Lerp(oldRaycastStart, raycastStart.position, lerpValue),
                    Vector3.Lerp(oldRaycastEnd, raycastEnd.position, lerpValue),
                    Color.red,
                    0.5f
                    );*/
                Vector3 startPoint = Vector3.Lerp(oldRaycastStart, raycastStart.position, lerpValue);
                Vector3 endPoint = Vector3.Lerp(oldRaycastEnd, raycastEnd.position, lerpValue);
                if (Physics.Linecast(
                        startPoint, 
                        endPoint - startPoint,
                        out RaycastHit raycastHit,
                        layerMask))
                {
                    raycastHit.collider.GetComponent<HurtCollider>()?.NotifyHit(this);
                }
            }
            
            
            oldRaycastStart = raycastStart.position;
            oldRaycastEnd = raycastEnd.position;
        }
    }

    internal override void PerformAttack()
    {
        oldRaycastStart = raycastStart.position;
        oldRaycastEnd = raycastEnd.position;
        remainingAttackDuration = attackDuration;
    }

    public float GetDamage()
    {
        return damage;
    }
}
