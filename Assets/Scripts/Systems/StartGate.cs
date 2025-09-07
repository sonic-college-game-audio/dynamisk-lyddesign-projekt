using UnityEngine;

public class StartGate : MonoBehaviour
{
    public DistanceToPath distanceToPath;

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
        
        distanceToPath.BeginTrackingDistanceToPath();
        Game.currentLevel.ReportEnterStartGate();
    }
}
