using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerUtility : MonoBehaviour
{
    private static TimerUtility _instance;
    private readonly List<Timer> timers = new List<Timer>();

    public static TimerUtility Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("TimerUtility");
                _instance = obj.AddComponent<TimerUtility>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    private void Update()
    {
        for (int i = timers.Count - 1; i >= 0; i--)
        {
            timers[i].Update(Time.deltaTime);
            if (timers[i].IsCompleted)
            {
                timers.RemoveAt(i);
            }
        }
    }

    public Timer CreateTimer(float duration, Action onComplete, bool startImmediately = false)
    {
        Timer timer = new Timer(duration, onComplete);

        // Register and start the timer if needed
        RegisterTimer(timer, startImmediate: startImmediately);

        return timer;
    }

    public void RegisterTimer(Timer timer, bool startImmediate = false)
    {
        // Automatically reset the timer if it's completed
        if (timer.IsCompleted)
        {
            timer.Reset();
        }

        // Add the timer to the active timers list if it's not already tracked
        if (!timers.Contains(timer))
        {
            timers.Add(timer);
        }

        // Start the timer immediately if requested
        if (startImmediate)
        {
            timer.Start();
        }
    }
}

public class Timer
{
    public float Duration { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsCompleted { get; private set; }

    private float elapsedTime;
    private Action onComplete;

    public Timer(float duration, Action onComplete)
    {
        this.Duration = duration;
        this.onComplete = onComplete;
        this.IsRunning = false;
        this.IsCompleted = false;
        this.elapsedTime = 0f;
    }

    public void Start()
    {
        if (!IsRunning)
        {
            IsRunning = true;
            IsCompleted = false;
        }
    }

    public void Pause()
    {
        if (IsRunning)
        {
            IsRunning = false;
        }
    }

    public void Reset()
    {
        elapsedTime = 0f;
        IsRunning = false;
        IsCompleted = false;
    }

    public void Update(float deltaTime)
    {
        if (!IsRunning || IsCompleted) return;

        elapsedTime += deltaTime;
        if (elapsedTime >= Duration)
        {
            IsCompleted = true;
            IsRunning = false;
            onComplete?.Invoke();
        }
    }
}
