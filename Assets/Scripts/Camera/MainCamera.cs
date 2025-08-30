using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Camera camera;

    private void Reset()
    {
        camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        CameraManager.RegisterMainCamera(camera);
    }
    
    private void OnDisable()
    {
        CameraManager.DeregisterMainCamera(camera);
    }
}