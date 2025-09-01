using FMODUnity;
using UnityEngine;

public class UnlockCutscene : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public GameObject cinematicBars;
    public Material activeProgressIndicatorMaterial;
    public Material openGateMaterial;
    public Renderer gateRenderer;
    public Collider gateCollider;
    public Renderer[] progressIndicators;
    
    [Header("Settings")]
    public float startDistance;
    public float targetDistance;
    public float duration;
    public AnimationCurve curve;

    [Header("Audio")]
    public EventReference cutsceneEvent;
    public EventReference progressIndicatorEvent;
    public EventReference gateUnlocksEvent;
    
    private int pickupCount;
    
    private void Start()
    {
        Pickup.OnPickedUp += OnPickedUp;
        
        cameraTransform.gameObject.SetActive(false);
        cinematicBars.SetActive(false);
    }
    
    private void OnDestroy()
    {
        Pickup.OnPickedUp -= OnPickedUp;
    }

    private void OnPickedUp()
    {
        PlayCutscene().Run();
    }

    private async Awaitable PlayCutscene()
    {
        await Awaitable.WaitForSecondsAsync(0.5f);
        
        RuntimeManager.PlayOneShot(cutsceneEvent);
        cameraTransform.gameObject.SetActive(true);
        cinematicBars.SetActive(true);
        Time.timeScale = 0;
        
        Vector3 startCameraLocalPosition = cameraTransform.localPosition;
        startCameraLocalPosition.z = startDistance;

        Vector3 targetCameraLocalPosition = cameraTransform.localPosition;
        targetCameraLocalPosition.z = targetDistance;

        float t = 0;
        while (t < 1)
        {
            if (t > 0.6f && progressIndicators[pickupCount].sharedMaterial != activeProgressIndicatorMaterial)
            {
                RuntimeManager.PlayOneShot(progressIndicatorEvent);
                progressIndicators[pickupCount].sharedMaterial = activeProgressIndicatorMaterial;
                
                if (pickupCount == 2)
                {
                    RuntimeManager.PlayOneShot(gateUnlocksEvent);
                    _ = ChangeGateMaterial();
                    gateCollider.isTrigger = true;
                }
            }
            
            t += Time.unscaledDeltaTime / duration;
            float f = curve.Evaluate(t);
            cameraTransform.localPosition = Vector3.Lerp(startCameraLocalPosition, targetCameraLocalPosition, f);
            await Awaitable.NextFrameAsync();
        }
        
        pickupCount++;
        if (pickupCount == 3)
        {
            await AwaitableUtility.WaitForSecondsRealtimeAsync(1);
        }
        
        cameraTransform.gameObject.SetActive(false);
        cinematicBars.SetActive(false);
        Time.timeScale = 1;
    }

    private async Awaitable ChangeGateMaterial()
    {
        Material a = gateRenderer.sharedMaterial;
        Material b = openGateMaterial;
        Material transition = Instantiate(a);
        gateRenderer.sharedMaterial = transition;

        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime;
            transition.Lerp(a, b, t);
            await Awaitable.NextFrameAsync();
        }
    }
}