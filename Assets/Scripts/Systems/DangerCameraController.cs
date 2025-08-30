using UnityEngine;

public class DangerCameraController : MonoBehaviour
{
    public DistanceToPath distanceToPath;

    private void Update()
    {
        Camera playerCamera = CameraManager.MainCamera;
        
        if (distanceToPath.IsOutsideSafeArea)
        {
            playerCamera.fieldOfView = Mathf.MoveTowards(playerCamera.fieldOfView, 63, Time.deltaTime * 5 / 0.15f);
        }
        else
        {
            playerCamera.fieldOfView = Mathf.MoveTowards(playerCamera.fieldOfView, 60, Time.deltaTime * 5 / 0.3f);
        }
    }
}