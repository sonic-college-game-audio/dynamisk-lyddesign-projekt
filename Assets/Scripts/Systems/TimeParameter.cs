using FMODUnity;
using UnityEngine;

public class TimeParameter : MonoBehaviour
{
    [ParamRef]
    public string timeParameter;
    public float maxTime;

    private float time;
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
        if (!shouldIncrementTimeParameter)
        {
            return;
        }

        time += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(time / maxTime);
        RuntimeManager.StudioSystem.setParameterByName(timeParameter, normalizedTime);
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