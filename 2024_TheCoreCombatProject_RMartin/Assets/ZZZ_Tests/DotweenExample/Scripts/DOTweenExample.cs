using UnityEngine;
using DG.Tweening;
public class DOTweenExample : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //all the same, different ways to move the object
        /*DOTween.To(() => transform.position,
            (v)=> transform.position = v,
            Vector3.up * 15f, 10f);*/
        Tween tween = transform.DOMove(Vector3.up * 15, 10f);
        DOTween.Sequence().Append(tween).SetEase(Ease.OutQuint)
            .Append(transform.DOMove(Vector3.left * 2f, 2f)).SetRelative()
            .Append(transform.DOMove(Vector3.right * 3f, 2f).SetEase(Ease.OutBounce));
        //transform.DOMove(Vector3.up * 15, 10f);
    }
}
