using FMODUnity;
using UnityEngine;

public class FootstepsAudio : MonoBehaviour
{
    public CharacterController characterController;
    public float distanceBetweenFootsteps;
    public EventReference footstepEvent;
    public EventReference landingEvent;

    private float distanceSinceLastFootstep;
    private Vector3 lastPosition;
    private bool wasGroundedLastFrame;

    private void Reset()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        lastPosition = transform.position;
        wasGroundedLastFrame = true;
    }

    private void Update()
    {
        PlayFootstepEvent();
        PlayLandingEvent();
    }

    private void PlayFootstepEvent()
    {
        Vector3 position = transform.position;
        
        if (!characterController.isGrounded)
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
        if (characterController.isGrounded && !wasGroundedLastFrame)
        {
            RuntimeManager.PlayOneShot(landingEvent);
        }

        wasGroundedLastFrame = characterController.isGrounded;
    }
}