using FMODUnity;
using UnityEngine;

public class KillPlayerAfterTime : MonoBehaviour
{
    [ParamRef]
    public string timeParameter;
    public float time;

    private float timePassed;
    private bool shouldIncrementTimeParameter;
    
    private void Start()
    {
        Game.currentLevel.OnEnterStartGate += OnEnterStartGate;
        Game.currentLevel.OnEnterEndGate += OnEnterEndGate;
    }

    private void OnDestroy()
    {
        Game.currentLevel.OnEnterStartGate -= OnEnterStartGate;
        Game.currentLevel.OnEnterEndGate -= OnEnterEndGate;
    }

    private void Update()
    {
        if (!Game.currentLevel.PlayerIsAlive)
        {
            return;
        }
        
        if (shouldIncrementTimeParameter)
        {
            timePassed += Time.deltaTime;
        }

        float normalizedTime = Mathf.Clamp01(timePassed / time);
        RuntimeManager.StudioSystem.setParameterByName(timeParameter, normalizedTime);

        if (timePassed > time)
        {
            Game.currentLevel.ReportPlayerKilled();
            shouldIncrementTimeParameter = false;
        }
    }

    private void OnEnterStartGate()
    {
        shouldIncrementTimeParameter = true;
    }
    
    private void OnEnterEndGate()
    {
        shouldIncrementTimeParameter = false;
    }
}