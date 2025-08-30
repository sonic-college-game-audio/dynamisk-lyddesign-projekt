using UnityEngine;

public class EndGate : MonoBehaviour
{
    public DistanceToPath distanceToPath;
    public float timeUntilRestart;

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
        
        distanceToPath.enabled = false;
        hasBeenTriggered = true;

        EndGameAfterDelay().Run();
    }

    private async Awaitable EndGameAfterDelay()
    {
        await Awaitable.WaitForSecondsAsync(timeUntilRestart);
        Game.ReportGameWon();
    }
}
