using UnityEngine;

[ExecuteAlways]
public class SendUnscaledTimeToShaders : MonoBehaviour
{
    private static readonly int UnscaledTime = Shader.PropertyToID("_UnscaledTime");

    private void Update()
    {
        Shader.SetGlobalFloat(UnscaledTime, Time.unscaledTime);
    }
}