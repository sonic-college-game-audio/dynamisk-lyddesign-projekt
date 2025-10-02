using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class CameraManager
{
    public static Camera MainCamera { get; private set; }

    private static List<Camera> cameraStack;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        MainCamera = null;
        cameraStack = new List<Camera>();
    }
    
    public static void RegisterMainCamera(Camera camera)
    {
        MainCamera = camera;
        ApplyCameraStackToMainCamera();
    }

    public static void DeregisterMainCamera(Camera camera)
    {
        if (MainCamera == camera)
        {
            MainCamera = null;
        }
    }

    public static void RegisterOverlayCamera(Camera camera)
    {
        camera.allowMSAA = MainCamera.allowMSAA;
        camera.allowDynamicResolution = MainCamera.allowDynamicResolution;
        camera.targetTexture = MainCamera.targetTexture;
        camera.clearFlags = MainCamera.clearFlags;
        
        cameraStack.Add(camera);
        ApplyCameraStackToMainCamera();
    }
    
    public static void DeregisterOverlayCamera(Camera camera)
    {
        cameraStack.Remove(camera);
        ApplyCameraStackToMainCamera();
    }

    private static void ApplyCameraStackToMainCamera()
    {
        if (!MainCamera)
        {
            return;
        }
        
        UniversalAdditionalCameraData cameraData = MainCamera.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Clear();
        cameraData.cameraStack.AddRange(cameraStack);
    }
}