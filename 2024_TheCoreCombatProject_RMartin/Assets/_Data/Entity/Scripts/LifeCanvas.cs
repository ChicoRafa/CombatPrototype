using System;
using UnityEngine;
using UnityEngine.UI;

public class LifeCanvas : MonoBehaviour
{
    [SerializeField] Image lifeBar;
    [SerializeField] EntityLife entityLife;

    private void OnEnable()
    {
        entityLife.onLifeChanged.AddListener(OnLifeChanged);
    }
    
    private void OnDisable()
    {
        entityLife.onLifeChanged.RemoveListener(OnLifeChanged);
    }

    private void OnLifeChanged(float newLifeValue)
    {
        lifeBar.fillAmount = newLifeValue;
    }
}
