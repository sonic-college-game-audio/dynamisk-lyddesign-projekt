using UnityEngine;
using UnityEngine.Rendering;

public class DangerPostProcessing : MonoBehaviour
{
    public DistanceToPath distanceToPath;
    public Volume postProcessingVolume;
    public AnimationCurve curve;

    private void Update()
    {
        float normalizedTimeOutsideSafeArea = distanceToPath.NormalizedTimeOutsideSafeArea;
        postProcessingVolume.weight = curve.Evaluate(normalizedTimeOutsideSafeArea);
    }
}