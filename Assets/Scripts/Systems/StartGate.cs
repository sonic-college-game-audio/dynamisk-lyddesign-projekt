using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class StartGate : MonoBehaviour
{
    public DistanceToPath distanceToPath;

    [Header("Audio")]
    public EventReference enterStartGateEvent;
    public EventReference introEvent;
    public EventReference ambienceEvent;
    
    private bool hasBeenTriggered;
    private EventInstance introInstance;
    private EventInstance ambienceInstance;

    private void Start()
    {
        introInstance = RuntimeManager.CreateInstance(introEvent);
        introInstance.start();
    }

    private void OnDestroy()
    {
        if (introInstance.isValid())
        {
            introInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            introInstance.release();
        }
        
        if (ambienceInstance.isValid())
        {
            ambienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            ambienceInstance.release();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered)
        {
            return;
        }
        
        if (!other.CompareTag("Player"))
        {
            return;
        }

        introInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        introInstance.release();
        RuntimeManager.PlayOneShot(enterStartGateEvent);
        ambienceInstance = RuntimeManager.CreateInstance(ambienceEvent);
        ambienceInstance.start();
        
        distanceToPath.BeginTrackingDistanceToPath();
        Game.currentLevel.ReportEnterStartGate();
    }
}
