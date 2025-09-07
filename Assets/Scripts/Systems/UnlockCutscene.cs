using FMODUnity;
using UnityEngine;

public class UnlockCutscene : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public GameObject cinematicBars;
    public Material activeUnlockStepMaterial;
    public Material openGateMaterial;
    public Renderer gateRenderer;
    public Collider gateCollider;
    public GameObject pickupVisualsPrefab;
    public Renderer[] unlockSteps;
    
    [Header("Settings")]
    public float startDistance;
    public float targetDistance;
    public float duration;
    public AnimationCurve curve;

    [Header("Audio")]
    public EventReference cutsceneEvent;
    public EventReference unlockStepEvent;
    public EventReference gateUnlocksEvent;
    
    private int pickupCount;
    
    private void Start()
    {
        Pickup.OnArrivedAtGate += OnArrivedAtGate;
        
        cameraTransform.gameObject.SetActive(false);
        cinematicBars.SetActive(false);
    }
    
    private void OnDestroy()
    {
        Pickup.OnArrivedAtGate -= OnArrivedAtGate;
    }

    private void OnArrivedAtGate()
    {
        PlayCutsceneAsync().Run();
    }

    private async Awaitable PlayCutsceneAsync()
    {
        Game.currentLevel.ReportCutsceneStart();
        
        RuntimeManager.PlayOneShot(cutsceneEvent);
        cameraTransform.gameObject.SetActive(true);
        cinematicBars.SetActive(true);
        
        Game.DisablePlayerInput();
        
        Vector3 startCameraLocalPosition = cameraTransform.localPosition;
        startCameraLocalPosition.z = startDistance;

        Vector3 targetCameraLocalPosition = cameraTransform.localPosition;
        targetCameraLocalPosition.z = targetDistance;

        GameObject pickupInstance = Instantiate(pickupVisualsPrefab, unlockSteps[pickupCount].transform.parent);
        pickupInstance.transform.localPosition = Vector3.back * 5;
        pickupInstance.transform.localScale = Vector3.one * 0.5f;
        
        float t = 0;
        while (t < 1)
        {
            float pickupTime = Mathf.Clamp01(t * 2);
            pickupInstance.transform.localPosition = Vector3.Lerp(Vector3.back * 5, Vector3.zero, curve.Evaluate(pickupTime));
            
            if (t > 0.6f && unlockSteps[pickupCount].sharedMaterial != activeUnlockStepMaterial)
            {
                RuntimeManager.PlayOneShot(unlockStepEvent);
                unlockSteps[pickupCount].sharedMaterial = activeUnlockStepMaterial;
                
                if (pickupCount == 2)
                {
                    RuntimeManager.PlayOneShot(gateUnlocksEvent);
                    _ = ChangeGateMaterialAsync();
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
        Game.EnablePlayerInput();
        Game.currentLevel.ReportCutsceneEnd();
    }

    private async Awaitable ChangeGateMaterialAsync()
    {
        Material a = gateRenderer.sharedMaterial;
        Material b = openGateMaterial;
        Material transitionMaterial = Instantiate(a);
        gateRenderer.sharedMaterial = transitionMaterial;

        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime;
            transitionMaterial.Lerp(a, b, t);
            await Awaitable.NextFrameAsync();
        }
    }
}