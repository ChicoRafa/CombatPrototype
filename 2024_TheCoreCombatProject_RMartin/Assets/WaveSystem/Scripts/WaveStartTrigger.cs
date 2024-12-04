using System;
using UnityEngine;
using UnityEngine.Events;

public class WaveStartTrigger : MonoBehaviour
{
    public UnityEvent onTriggered;
    private const string TAG_PLAYER = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TAG_PLAYER)) onTriggered.Invoke();
    }
}
