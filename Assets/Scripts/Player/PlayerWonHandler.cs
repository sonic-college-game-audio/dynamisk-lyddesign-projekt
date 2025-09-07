using UnityEngine;

public class PlayerWonHandler : MonoBehaviour
{
    public PlayerMovement playerMovement;
    
    private void Start()
    {
        Game.currentLevel.OnPlayerWon += OnPlayerWon;
    }

    private void OnPlayerWon()
    {
        OnPlayerWonAsync().Run();
    }

    private async Awaitable OnPlayerWonAsync()
    {
        await Awaitable.WaitForSecondsAsync(1);
        playerMovement.enabled = false;
    }
}