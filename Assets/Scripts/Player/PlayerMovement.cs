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
    public float jumpBufferDuration = 0.125f;
    public float coyoteTime = 0.125f;

    [Header("Audio")]
    public EventReference jumpEvent;

    private InputAction moveInputAction;
    private InputAction jumpInputAction;
    private Vector2 smoothedInput;
    private Vector2 smoothingVelocity;
    private Vector3 verticalVelocity;
    private bool debugHovering;
    private float lastJumpInput = float.NegativeInfinity;
    private float lastGroundedTime;

    public bool IsJumping { get; private set; }
    
    private void Reset()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        moveInputAction = InputSystem.actions.FindAction("Move");
        jumpInputAction = InputSystem.actions.FindAction("Jump");
        characterController.minMoveDistance = 0;
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

        if (characterController.isGrounded)
        {
            IsJumping = false;
            lastGroundedTime = Time.time;
        }
        
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
        if (jumpInputAction.WasPerformedThisFrame())
        {
            lastJumpInput = Time.time;
        }

        float timeSinceLastJumpInput = lastJumpInput < 0 ? float.PositiveInfinity : Time.time - lastJumpInput;
        float timeSinceLastGrounded = Time.time - lastGroundedTime;
        
        if (!IsJumping && timeSinceLastGrounded < coyoteTime && timeSinceLastJumpInput < jumpBufferDuration)
        {
            // Jump!
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
            RuntimeManager.PlayOneShot(jumpEvent);
            IsJumping = true;
        }
        else
        {
            if (verticalVelocity.y > 0)
            {
                // We are ascending
                verticalVelocity += Physics.gravity * (ascentGravityMultiplier * Time.deltaTime);
            }
            else if (!characterController.isGrounded)
            {
                // We are descending
                verticalVelocity += Physics.gravity * (descentGravityMultiplier * Time.deltaTime);
            }
            else
            {
                // The controller is experiencing the force of gravity but without acceleration
                verticalVelocity = Physics.gravity * Time.deltaTime;
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