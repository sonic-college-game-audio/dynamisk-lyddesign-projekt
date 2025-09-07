using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlayerKilledHandler : MonoBehaviour
{
    public CharacterController characterController;
    public PlayerMovement playerMovement;
    public PlayerMovement playerLook;
    public Rigidbody cameraRigidbody;
    public float fallForce;

    [Header("Audio")]
    public EventReference deathEvent;
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
            killedSnapshotEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
            killedSnapshotEventInstance.release();
        }
    }

    private void OnPlayerKilled()
    {
        RuntimeManager.PlayOneShot(deathEvent);
        killedSnapshotEventInstance = RuntimeManager.CreateInstance(killedSnapshot);
        killedSnapshotEventInstance.start();

        playerMovement.enabled = false;
        playerLook.enabled = false;
        
        cameraRigidbody.isKinematic = false;
        cameraRigidbody.AddTorque(transform.right * fallForce, ForceMode.Impulse);
    }
}
