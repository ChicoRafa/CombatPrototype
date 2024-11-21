using System;
using UnityEngine;
using UnityEngine.Events;

public class Wave : MonoBehaviour
{
    private UnityEvent onWaveEnded;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        gameObject.SetActive(false);
    }

    private void Update()
    {
        EndWave();
    }
    public void EndWave()
    {
        if (transform.childCount == 0)
        {
            gameObject.SetActive(false);
            onWaveEnded.Invoke();
        }
    }
}
