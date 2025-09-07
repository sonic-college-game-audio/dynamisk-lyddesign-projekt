using System;
using UnityEngine;

public class Level : MonoBehaviour
{
    public event Action OnPlayerKilled;
    public event Action OnPlayerWon;
    public event Action OnEnterStartGate;
    public event Action OnEnterEndGate;
    
    public float waitAfterKilled;
    public float waitAfterWon;

    private void Awake()
    {
        Game.currentLevel = this;
    }

    public void ReportEnterStartGate()
    {
        OnEnterStartGate?.Invoke();
    }

    public void ReportEnterEndGate()
    {
        OnEnterEndGate?.Invoke();
        ReportPlayerWonAsync().Run();
    }
    
    public void ReportPlayerKilled()
    {
        ReportPlayerKilledAsync().Run();
    }
    
    private async Awaitable ReportPlayerKilledAsync()
    {
        OnPlayerKilled?.Invoke();
        await Awaitable.WaitForSecondsAsync(waitAfterKilled);
        Game.ReloadLevel();
    }
    
    private async Awaitable ReportPlayerWonAsync()
    {
        OnPlayerWon?.Invoke();
        await Awaitable.WaitForSecondsAsync(waitAfterWon);
        Game.ReloadLevel();
    }
}