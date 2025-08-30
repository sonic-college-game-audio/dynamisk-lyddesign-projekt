using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public Transform cameraTransform;
    public float sensitivity;
    
    private InputAction lookInputAction;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        lookInputAction = InputSystem.actions.FindAction("Look");

        Game.OnGameLost += () => enabled = false;
    }

    private void Update()
    {
        Vector2 input = lookInputAction.ReadValue<Vector2>() * sensitivity;
        
        transform.Rotate(transform.up, input.x, Space.World);

        Vector3 cameraEulerAngles = cameraTransform.localEulerAngles;
        cameraEulerAngles.x = Mathf.DeltaAngle(0, cameraEulerAngles.x);
        cameraEulerAngles.x -= input.y;
        cameraEulerAngles.x = Mathf.Clamp(cameraEulerAngles.x, -90, 90);
        cameraTransform.localEulerAngles = cameraEulerAngles;
    }
}