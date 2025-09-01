using System;
using FMODUnity;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public static event Action OnPickedUp;
    
    public Transform visual;
    public Collider trigger;
    public Light pointLight;
    
    [Header("Pickup Animation")]
    public AnimationCurve scaleWhenPickedUpCurve;
    public float scaleAnimationDuration;

    [Header("Audio")]
    public EventReference pickupEvent;
    public StudioEventEmitter loopEmitter;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        trigger.enabled = false;
        RuntimeManager.PlayOneShot(pickupEvent);
        loopEmitter.Stop();
        AnimatePickup().Run();
    }

    private async Awaitable AnimatePickup()
    {
        float t = 0;
        float startIntensity = pointLight.intensity;
        
        while (t < 1)
        {
            t += Time.deltaTime / scaleAnimationDuration;
            visual.localScale = Vector3.one * scaleWhenPickedUpCurve.Evaluate(t);
            pointLight.intensity = startIntensity * (1 - t);
            await Awaitable.NextFrameAsync();
        }
        
        OnPickedUp?.Invoke();
    }
}
