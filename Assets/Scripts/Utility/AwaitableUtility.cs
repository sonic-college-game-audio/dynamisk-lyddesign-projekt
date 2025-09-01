using UnityEngine;

public static class AwaitableUtility
{
    public static Awaitable Run(this Awaitable self)
    {
        self.GetAwaiter().OnCompleted(() => self.GetAwaiter().GetResult());
        return self;
    }

    public static async Awaitable WaitForSecondsRealtimeAsync(float seconds)
    {
        float targetTime = Time.unscaledTime + seconds;
        while (Time.unscaledTime < targetTime)
        {
            await Awaitable.NextFrameAsync();
        }
    }
}