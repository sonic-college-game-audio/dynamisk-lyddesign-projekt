using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlayerKilledHandler : MonoBehaviour
{
    public CharacterController characterController;
    public Rigidbody cameraRigidbody;
    public float fallForce;

    [Header("Audio")]
    public EventReference killedEvent;
    public EventReference timeRanOutEvent;
    public EventReference killedSnapshot;

    private EventInstance killedSnapshotEventInstance;
    
    private void Start()
    {
        Physics.IgnoreCollision(characterController, cameraRigidbody.GetComponent<Collider>());
        Game.currentLevel.OnPlayerKilled += OnPlayerKilled;
    }

    private void OnDestroy()
    {
        if (killedSnapshotEventInstance.isValid())
        {
            killedSnapshotEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            killedSnapshotEventInstance.release();
        }
    }

    private void OnPlayerKilled(Level.CauseOfDeath causeOfDeath)
    {
        switch (causeOfDeath)
        {
            case Level.CauseOfDeath.Killed:
                RuntimeManager.PlayOneShot(killedEvent);
                break;
            case Level.CauseOfDeath.TimeRanOut:
                RuntimeManager.PlayOneShot(timeRanOutEvent);
                break;
        }
        
        killedSnapshotEventInstance = RuntimeManager.CreateInstance(killedSnapshot);
        killedSnapshotEventInstance.start();

        Game.DisablePlayerInput();
        
        cameraRigidbody.isKinematic = false;
        cameraRigidbody.AddTorque(transform.right * fallForce, ForceMode.Impulse);
    }
}
