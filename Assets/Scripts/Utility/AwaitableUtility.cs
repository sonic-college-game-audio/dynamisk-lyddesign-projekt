using UnityEngine;

public static class AwaitableUtility
{
    public static Awaitable Run(this Awaitable self)
    {
        self.GetAwaiter().OnCompleted(() => self.GetAwaiter().GetResult());
        return self;
    }
}