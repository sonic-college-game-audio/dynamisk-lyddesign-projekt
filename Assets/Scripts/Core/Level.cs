using System;
using UnityEngine;

public class Level : MonoBehaviour
{
    public event Action OnPlayerKilled;
    public event Action OnPlayerWon;
    public event Action OnEnterStartGate;
    public event Action OnEnterEndGate;
    public event Action OnGateUnlockStep;
    
    public Transform playerTransform;
    public float waitAfterKilled;
    public float waitAfterWon;

    private Pickup[] pickups;
    private int numberOfPickupsDelivered;
    
    public bool IsShowingCutscene { get; private set; }
    public int NumberOfPickups => pickups.Length;
    
    private void Awake()
    {
        Game.currentLevel = this;
        playerTransform = GameObject.FindWithTag("Player").transform;
        pickups = FindObjectsByType<Pickup>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Pickup pickup in pickups)
        {
            pickup.OnDelivered += OnPickupDelivered;
        }
    }

    private void OnPickupDelivered()
    {
        numberOfPickupsDelivered++;
        OnGateUnlockStep?.Invoke();

        if (numberOfPickupsDelivered == NumberOfPickups)
        {
            FindFirstObjectByType<EndGate>().Open();
        }
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