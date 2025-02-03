using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    [SerializeField] Transform objectsToActivateOnStartParent;
    Wave[] waves;
    EventStartTrigger eventStartTrigger;
    public UnityEvent onWavesFinished;
    
    bool alreadyStarted = false;
    int currentWaveIndex = 0;
    private void Awake()
    {
        waves = GetComponentsInChildren<Wave>(true);
        eventStartTrigger = GetComponentInChildren<EventStartTrigger>();
    }

    private void OnEnable()
    {
        foreach (Wave wave in waves)
        {
            wave.onWaveEnded.AddListener(OnWaveEnded);
        }
        eventStartTrigger.onTriggered.AddListener(OnWaveStartTriggered);
    }
    
    private void OnDisable()
    {
        foreach (Wave wave in waves)
        {
            wave.onWaveEnded.RemoveListener(OnWaveEnded);
        }
        eventStartTrigger.onTriggered.RemoveListener(OnWaveStartTriggered);
    }

    private void OnWaveStartTriggered()
    {
        if (!alreadyStarted)
        {
            alreadyStarted = true;
            objectsToActivateOnStartParent.gameObject.SetActive(true);
            waves[currentWaveIndex].gameObject.SetActive(true);
        }
    }

    private void OnWaveEnded()
    {
        currentWaveIndex++;
        if (currentWaveIndex < waves.Length)
        {
            waves[currentWaveIndex].gameObject.SetActive(true);
        }
        else
        {
            objectsToActivateOnStartParent.gameObject.SetActive(false);
            onWavesFinished.Invoke();
        }
    }
}
