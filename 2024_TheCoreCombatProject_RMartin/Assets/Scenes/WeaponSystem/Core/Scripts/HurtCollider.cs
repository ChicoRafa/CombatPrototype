using UnityEngine;
using UnityEngine.Events;

public class HurtCollider : MonoBehaviour
{
    public UnityEvent OnHit;
    public void NotifyHit(IHitter hitter)
    {
        //Calls all the functions that are subscribed to the OnHit event, in this case, SetActive(false) on the GameObject
        Debug.Log(hitter.GetDamage());
        OnHit.Invoke();
    }
}
