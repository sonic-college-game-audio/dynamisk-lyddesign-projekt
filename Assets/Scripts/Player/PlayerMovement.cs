using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

[SelectionBase]
public class PlayerMovement : MonoBehaviour
{
    public CharacterController characterController;
    
    [Header("Movement")]
    public float movementSpeed = 6;
    public float smoothingTime = 0.08f;
    public float debugSprintSpeed = 32;
    
    [Header("Jumping")]
    public float jumpHeight = 2;
    public float ascentGravityMultiplier = 1.5f;
    public float descentGravityMultiplier = 2;

    [Header("Audio")]
    public EventReference jumpEvent;

    private InputAction moveInputAction;
    private InputAction jumpInputAction;
    private Vector2 smoothedInput;
    private Vector2 smoothingVelocity;
    private Vector3 verticalVelocity;
    private bool debugHovering;

    private void Reset()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        moveInputAction = InputSystem.actions.FindAction("Move");
        jumpInputAction = InputSystem.actions.FindAction("Jump");
    }

    private void Update()
    {
#if PLATFORM_STANDALONE_OSX
        debugHovering = Keyboard.current.capsLockKey.isPressed;
#else
        if (Keyboard.current.capsLockKey.wasPressedThisFrame)
        {
            debugHovering = !debugHovering;
        }
#endif

        Vector3 motion = CalculateMovement();
        
        if (!debugHovering)
        {
            motion += CalculateJump();
        }
        
        characterController.Move(motion * Time.deltaTime);
    }

    private Vector3 CalculateMovement()
    {
        Vector2 input = moveInputAction.ReadValue<Vector2>();
        smoothedInput = Vector2.SmoothDamp(smoothedInput, input, ref smoothingVelocity, smoothingTime);
        
        Vector3 direction = transform.TransformDirection(smoothedInput.x, 0, smoothedInput.y);
        direction = Vector3.ClampMagnitude(direction, 1);

        float speed = debugHovering ? debugSprintSpeed : movementSpeed;
        Vector3 motion = direction * speed;
        return motion;
    }

    private Vector3 CalculateJump()
    {
        if (characterController.isGrounded && jumpInputAction.WasPerformedThisFrame())
        {
            if (jumpInputAction.WasPerformedThisFrame())
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
                RuntimeManager.PlayOneShot(jumpEvent);
            }
        }
        else
        {
            if (verticalVelocity.y < 0)
            {
                verticalVelocity += Physics.gravity * (descentGravityMultiplier * Time.deltaTime);
            }
            else
            {
                verticalVelocity += Physics.gravity * (ascentGravityMultiplier * Time.deltaTime);
            }
        }

        return verticalVelocity;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.rigidbody || hit.rigidbody.isKinematic)
        {
            return;
        }

        if (hit.moveDirection.y < -0.3f)
        {
            return;
        }
        
        const float power = 5;
        Vector3 force = -hit.normal * hit.moveLength * power;
        force.y = 0;
        
        hit.rigidbody.AddForceAtPosition(force, hit.point, ForceMode.Impulse);
    }
}