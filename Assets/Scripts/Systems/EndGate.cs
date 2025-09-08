using FMODUnity;
using UnityEngine;

public class EndGate : MonoBehaviour
{
    public DistanceToPath distanceToPath;
    public Material openGateMaterial;
    public Renderer gateRenderer;
    public Collider gateCollider;
    public AnimationCurve materialTransitionCurve;
    public EventReference gateUnlocksEvent;
    
    private bool hasBeenTriggered;
    
    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered)
        {
            return;
        }
        
        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        Game.currentLevel.ReportEnterEndGate();
        distanceToPath.enabled = false;
        hasBeenTriggered = true;
    }

    public void Open()
    {
        gateCollider.isTrigger = true;
        OpenAsync().Run();
    }

    private async Awaitable OpenAsync()
    {
        await Awaitable.WaitForSecondsAsync(1.8f);
        
        RuntimeManager.PlayOneShot(gateUnlocksEvent);
        ChangeGateMaterialAsync().Run();
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
            transitionMaterial.Lerp(a, b, materialTransitionCurve.Evaluate(t));
            await Awaitable.NextFrameAsync();
            t += Time.unscaledDeltaTime / 1.5f;
        }

        gateRenderer.sharedMaterial = openGateMaterial;
        Destroy(transitionMaterial);
    }
}
