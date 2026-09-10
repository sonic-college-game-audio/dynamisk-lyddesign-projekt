using FMODUnity;
using UnityEngine;

public class FootstepsAudio : MonoBehaviour
{
    public CharacterController characterController;
    public PlayerMovement playerMovement;
    public float distanceBetweenFootsteps;
    public EventReference footstepEvent;
    public EventReference landingEvent;

    private float distanceSinceLastFootstep;
    private Vector3 lastPosition;
    private float airTime;
    
    private void OnValidate()
    {
        characterController = characterController ? characterController : GetComponent<CharacterController>();
        playerMovement = playerMovement ? playerMovement : GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        PlayFootstepEvent();
        PlayLandingEvent();
        
        if (!characterController.isGrounded)
        {
            airTime += Time.deltaTime;
        }
        else
        {
            airTime = 0;
        }
    }

    private void PlayFootstepEvent()
    {
        Vector3 position = transform.position;
        
        if (playerMovement.IsJumping || airTime > 0.5f)
        {
            lastPosition = position;
            distanceSinceLastFootstep = 0;
            return;
        }

        float distance = Vector3.Distance(position, lastPosition);
        distanceSinceLastFootstep += distance;
        lastPosition = position;

        if (distanceSinceLastFootstep >= distanceBetweenFootsteps)
        {
            RuntimeManager.PlayOneShot(footstepEvent);
            distanceSinceLastFootstep -= distanceBetweenFootsteps;
        }
    }
    
    private void PlayLandingEvent()
    {
        if (characterController.isGrounded && airTime > 0.5f)
        {
            RuntimeManager.PlayOneShot(landingEvent);
        }
    }
}