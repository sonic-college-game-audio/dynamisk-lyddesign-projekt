using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
public class PlaceOnSpline : MonoBehaviour
{
    public SplineContainer spline;
    public Mode mode;
    public float distance;

    private void Update()
    {
        if (Application.isPlaying)
        {
            return;
        }
        
        if (!spline)
        {
            return;
        }

        distance = Mathf.Max(0, distance);
        
        float fromT = mode switch
        {
            Mode.FromBeginning => 0,
            Mode.FromEnd => 1,
            _ => throw new ArgumentException()
        };

        float sampleDistance = mode switch
        {
            Mode.FromBeginning => distance,
            Mode.FromEnd => -distance, 
            _ => throw new ArgumentException()
        };
        
        spline.Spline.GetPointAtLinearDistance(fromT, sampleDistance, out float t);
        spline.Evaluate(t, out float3 position, out float3 tangent, out float3 upVector);
        transform.position = position;
        transform.forward = tangent;
    }

    public enum Mode
    {
        FromBeginning,
        FromEnd
    }
}