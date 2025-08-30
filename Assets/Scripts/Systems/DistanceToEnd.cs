using FMODUnity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class DistanceToEnd : MonoBehaviour
{
    public SplineContainer spline;
    [ParamRef] public string distanceToEndParameter;

    private Transform playerTransform;

    private void Start()
    {
        if (!spline)
        {
            enabled = false;
            return;
        }
        
        GameObject player = GameObject.FindWithTag("Player");

        if (!player)
        {
            Debug.LogError("Could not find game object with tag \"Player\"");
            enabled = false;
            return;
        }
        
        playerTransform = player.transform;
        RuntimeManager.StudioSystem.setParameterByName(distanceToEndParameter, 1);
    }
    
    private void Update()
    {
        Vector3 point = playerTransform.position;
        float3 localPoint = spline.transform.InverseTransformPoint(point);
        SplineUtility.GetNearestPoint(spline.Spline, localPoint, out float3 _, out float t);
        RuntimeManager.StudioSystem.setParameterByName(distanceToEndParameter, Mathf.Clamp01(1 - t));
    }
}