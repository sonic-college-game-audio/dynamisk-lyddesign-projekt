using FMODUnity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class DistanceToPath : MonoBehaviour
{
    public SplineContainer spline;
    public float maxDistance;
    public float maxTimeOutsideSafeArea;
    
    [Header("Audio")]
    public EventReference enterSafeDistanceEvent;
    public EventReference exitSafeDistanceEvent;
    [ParamRef] public string distanceToPathParameter;
    [ParamRef] public string timeOutsidePathParameter;
    
    private Transform playerTransform;
    private float timeOutsideSafeArea;
    private bool wasOutsideSafeAreaLastFrame;
    private bool isTracking;
    
    public float NormalizedTimeOutsideSafeArea { get; private set; }
    public bool IsOutsideSafeArea { get; private set; }

    private void Reset()
    {
        spline = GetComponent<SplineContainer>();
    }

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
        RuntimeManager.StudioSystem.setParameterByName(timeOutsidePathParameter, 0);
        RuntimeManager.StudioSystem.setParameterByName(distanceToPathParameter, 0);
    }

    private void Update()
    {
        if (!isTracking)
        {
            return;
        }
        
        Vector3 playerPosition = playerTransform.position;
        Vector3 nearestPoint = GetNearestPointOnSpline(playerPosition);
        float distance = Vector3.Distance(playerPosition, nearestPoint);
        float normalizedDistance = Mathf.Clamp01(distance / maxDistance);
        RuntimeManager.StudioSystem.setParameterByName(distanceToPathParameter, normalizedDistance);

        IsOutsideSafeArea = distance > maxDistance;
        if (IsOutsideSafeArea)
        {
            if (!wasOutsideSafeAreaLastFrame)
            {
                RuntimeManager.PlayOneShot(exitSafeDistanceEvent);
            }
            
            timeOutsideSafeArea += Time.deltaTime;
            
            if (timeOutsideSafeArea > maxTimeOutsideSafeArea)
            {
                Game.ReportGameLost();
                isTracking = false;
            }
        }
        else
        {
            if (wasOutsideSafeAreaLastFrame)
            {
                RuntimeManager.PlayOneShot(enterSafeDistanceEvent);
            }
            
            timeOutsideSafeArea -= Time.deltaTime * 3;
            timeOutsideSafeArea = Mathf.Max(0, timeOutsideSafeArea);
        }

        NormalizedTimeOutsideSafeArea = Mathf.Clamp01(timeOutsideSafeArea / maxTimeOutsideSafeArea);
        RuntimeManager.StudioSystem.setParameterByName(timeOutsidePathParameter, NormalizedTimeOutsideSafeArea);
        wasOutsideSafeAreaLastFrame = IsOutsideSafeArea;
    }

    public void BeginTrackingDistanceToPath()
    {
        isTracking = true;
    }
    
    private Vector3 GetNearestPointOnSpline(Vector3 point)
    {
        float3 localPoint = spline.transform.InverseTransformPoint(point);
        SplineUtility.GetNearestPoint(spline.Spline, localPoint, out float3 nearestPoint, out _);
        return spline.transform.TransformPoint(nearestPoint);
    }
    
    private void OnDrawGizmos()
    {
        if (!spline)
        {
            return;
        }

        if (!playerTransform)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player)
            {
                playerTransform = player.transform;
            }
            else
            {
                return;
            }
        }
        
        Vector3 playerPosition = playerTransform.position;
        Vector3 nearestPoint = GetNearestPointOnSpline(playerPosition);
        Gizmos.DrawWireSphere(nearestPoint, 0.25f);
        Gizmos.DrawLine(playerPosition, nearestPoint);

        Gizmos.color = Color.red;
        
        int length = Mathf.FloorToInt(spline.CalculateLength());
        Vector3 lastLeft = Vector3.zero;
        Vector3 lastRight = Vector3.zero;
        for (int i = 0; i < length; i += 4)
        {
            CalculateLeftAndRight(i / (float)length, out Vector3 left, out Vector3 right);
            
            if (i > 0)
            {
                Gizmos.DrawLine(lastLeft, left);
                Gizmos.DrawLine(lastRight, right);
            }
            
            lastLeft = left;
            lastRight = right;
        }
        
        CalculateLeftAndRight(1, out Vector3 endLeft, out Vector3 endRight);
        Gizmos.DrawLine(lastLeft, endLeft);
        Gizmos.DrawLine(lastRight, endRight);
        
        void CalculateLeftAndRight(float t, out Vector3 left, out Vector3 right)
        {
            spline.Evaluate(t, out float3 position, out float3 tangent, out float3 _);
            Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;
            left = (Vector3)position + normal * maxDistance;
            right = (Vector3)position - normal * maxDistance;
        }
    }
}
