using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimerManager : MonoBehaviour
{
    private static TimerManager instance;
    private List<Timer> activeTimers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            activeTimers = new List<Timer>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void StartTimer(float duration, System.Action callback)
    {
        if (instance == null)
        {
            Debug.LogError("TimerManager not initialized.");
            return;
        }

        Timer timer = new Timer(duration, callback);
        instance.activeTimers.Add(timer);
        timer.Start(instance.StartCoroutine(timer.TimerCoroutine()));
    }

    private void Update()
    {
        // Check if any timers have finished and invoke their callbacks
        for (int i = activeTimers.Count - 1; i >= 0; i--)
        {
            Timer timer = activeTimers[i];
            if (timer.IsFinished)
            {
                timer.InvokeCallback();
                activeTimers.RemoveAt(i);
            }
        }
    }
}

public class Timer
{
    private float duration;
    private System.Action callback;
    private Coroutine coroutine;

    public bool IsFinished { get; private set; }

    public Timer(float duration, System.Action callback)
    {
        this.duration = duration;
        this.callback = callback;
        IsFinished = false;
    }

    public void Start(Coroutine coroutine)
    {
        this.coroutine = coroutine;
    }

    public IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(duration);

        IsFinished = true;
    }

    public void InvokeCallback()
    {
        callback?.Invoke();
    }
}
