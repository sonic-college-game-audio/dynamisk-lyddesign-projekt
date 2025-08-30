using FMODUnity;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Transform visual;
    public Collider trigger;
    
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

        while (t < 1)
        {
            t += Time.deltaTime / scaleAnimationDuration;
            visual.localScale = Vector3.one * scaleWhenPickedUpCurve.Evaluate(t);
            await Awaitable.NextFrameAsync();
        }
    }
}
