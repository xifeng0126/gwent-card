using UnityEngine;

public class Timer
{
    public static float timeLimit;
    private static float timer;

    public delegate void TimerCallback();

    private static TimerCallback callback;

    public static void StartTimer(float time, TimerCallback timerCallback)
    {
        timeLimit = time;
        callback = timerCallback;
        timer = 0f;
    }

    public static void Update()
    {
        if (timer < timeLimit)
        {
            timer += Time.deltaTime;
        }
        else
        {
            callback?.Invoke();
            // 计时器结束后，你可以选择停止计时器或执行其他操作
             StopTimer();
        }
    }

    private static void StopTimer()
    {
        timer = 0f;
        callback = null;
    }
}
