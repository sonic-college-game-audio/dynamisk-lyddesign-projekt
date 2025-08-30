using FMODUnity;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    public CharacterController characterController;
    public Rigidbody cameraRigidbody;
    public float fallForce;

    [Header("Audio")]
    public EventReference deathEvent;
    
    private void Start()
    {
        Physics.IgnoreCollision(characterController, cameraRigidbody.GetComponent<Collider>());
        Game.OnGameLost += OnGameLost;
    }

    private void OnGameLost()
    {
        RuntimeManager.PlayOneShot(deathEvent);
        
        cameraRigidbody.isKinematic = false;
        cameraRigidbody.AddTorque(transform.right * fallForce, ForceMode.Impulse);
    }
}
