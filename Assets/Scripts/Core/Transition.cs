using UnityEngine;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    public Image image;
    public float fadeOutTime = 1;
    public float fadeInTime = 1;
    public float waitTimeWhileCovered;

    private void Start()
    {
        Color color = image.color;
        color.a = 0;
        image.color = color;
    }

    public async Awaitable FadeOut()
    {
        Color color = image.color;
        float time = 0;
        while (time < fadeOutTime)
        {
            color.a = Mathf.Clamp01(time / fadeOutTime);
            image.color = color;
            await Awaitable.NextFrameAsync();
            time += Time.deltaTime;
        }
        
        color.a = 1;
        image.color = color;

        await Awaitable.WaitForSecondsAsync(waitTimeWhileCovered);
    }

    public async Awaitable FadeIn()
    {
        Color color = image.color;
        float time = 0;
        while (time < fadeInTime)
        {
            color.a = 1 - Mathf.Clamp01(time / fadeInTime);
            image.color = color;
            await Awaitable.NextFrameAsync();
            time += Time.deltaTime;
        }
        
        color.a = 0;
        image.color = color;
    }
}