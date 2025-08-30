using UnityEngine;

public class OverlayCamera : MonoBehaviour
{
    public Camera camera;

    private void Reset()
    {
        camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        CameraManager.RegisterOverlayCamera(camera);
    }
    
    private void OnDisable()
    {
        CameraManager.DeregisterOverlayCamera(camera);
    }
}