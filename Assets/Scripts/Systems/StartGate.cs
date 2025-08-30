using UnityEngine;

public class StartGate : MonoBehaviour
{
    public DistanceToPath distanceToPath;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        distanceToPath.BeginTrackingDistanceToPath();
    }
}
