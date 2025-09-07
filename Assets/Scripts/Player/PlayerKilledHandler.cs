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
    
    private void Start()
    {
        Physics.IgnoreCollision(characterController, cameraRigidbody.GetComponent<Collider>());
        Game.currentLevel.OnPlayerKilled += OnPlayerKilled;
    }

    private void OnPlayerKilled()
    {
        RuntimeManager.PlayOneShot(deathEvent);

        playerMovement.enabled = false;
        playerLook.enabled = false;
        
        cameraRigidbody.isKinematic = false;
        cameraRigidbody.AddTorque(transform.right * fallForce, ForceMode.Impulse);
    }
}
