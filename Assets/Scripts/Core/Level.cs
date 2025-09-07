using System;
using UnityEngine;

public class Level : MonoBehaviour
{
    public event Action OnPlayerKilled;
    public event Action OnPlayerWon;
    public event Action OnEnterStartGate;
    public event Action OnEnterEndGate;

    public Transform playerTransform;
    public float waitAfterKilled;
    public float waitAfterWon;
    
    public bool IsShowingCutscene { get; private set; }
    
    private void Awake()
    {
        Game.currentLevel = this;
        playerTransform = GameObject.FindWithTag("Player").transform;
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

    public void ReportCutsceneStart()
    {
        IsShowingCutscene = true;
    }

    public void ReportCutsceneEnd()
    {
        IsShowingCutscene = false;
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