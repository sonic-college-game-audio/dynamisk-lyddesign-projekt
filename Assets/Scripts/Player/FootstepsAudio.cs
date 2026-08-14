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

    private void Reset()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        lastPosition = transform.position;
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
        if (Time.timeSinceLevelLoad < 1f)
        {
            return;
        }
        
        if (characterController.isGrounded && characterController.velocity.y < -0.5f)
        {
            RuntimeManager.PlayOneShot(landingEvent);
        }
    }
}