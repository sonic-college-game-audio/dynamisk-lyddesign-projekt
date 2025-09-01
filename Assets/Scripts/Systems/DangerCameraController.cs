using UnityEngine;

public class DangerCameraController : MonoBehaviour
{
    public DistanceToPath distanceToPath;
    public float normalFieldOfView;
    public float dangerFieldOfView;
    public float transitionToDangerDuration;
    public float transitionToNormalDuration;
    
    private void Update()
    {
        Camera playerCamera = CameraManager.MainCamera;
        float delta = Mathf.Abs(normalFieldOfView - dangerFieldOfView);
        
        float fov = playerCamera.fieldOfView;
        if (distanceToPath.IsOutsideSafeArea)
        {
            float maxDelta = Time.deltaTime * delta / transitionToDangerDuration;
            playerCamera.fieldOfView = Mathf.MoveTowards(fov, dangerFieldOfView, maxDelta);
        }
        else
        {
            float maxDelta = Time.deltaTime * delta / transitionToNormalDuration;
            playerCamera.fieldOfView = Mathf.MoveTowards(fov, normalFieldOfView, maxDelta);
        }
    }
}