using UnityEngine;

public class EndGate : MonoBehaviour
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
        
        Game.currentLevel.ReportEnterEndGate();
        distanceToPath.enabled = false;
        hasBeenTriggered = true;
    }
}
