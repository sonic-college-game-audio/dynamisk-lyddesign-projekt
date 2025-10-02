using FMODUnity;
using UnityEngine;

public class PickupFoundParameter : MonoBehaviour
{
    [ParamRef] public string parameter;

    private int totalNumberOfPickups;
    private int numberOfPickupsPickedUp;
    
    private void Start()
    {
        Game.currentLevel.OnGateUnlockStep += OnGateUnlockStep;
        totalNumberOfPickups = FindObjectsByType<Pickup>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
    }

    private void OnGateUnlockStep(Pickup _)
    {
        numberOfPickupsPickedUp++;
        float normalized = (float) numberOfPickupsPickedUp / totalNumberOfPickups;
        RuntimeManager.StudioSystem.setParameterByName(parameter, normalized);
    }
}
