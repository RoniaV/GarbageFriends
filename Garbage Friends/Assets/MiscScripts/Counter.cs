using System.Collections;
using System.Collections.Generic;
using System;

public class Counter 
{
    #region Properties
    public float TimeToComplete { get { return timeToComplete; } }
    public float TimeLeft { get { return GetTimeLeft(); } }
    public bool Stopped { get { return stopped; } }
    public bool Finished { get { return UpdateCounter(); } }
    #endregion

    #region Fields
    private float timeToComplete;
    private float timeLeft;
    private DateTime lastDateTime;

    private bool stopped = true;
    private bool finished = false;
    #endregion

    #region Constructors
    public Counter() { }

    public Counter(float time, bool startNow = false)
    {
        timeToComplete = time;

        if (startNow)
            StartCounter();
    }
    #endregion

    #region Public Methods
    public void ResetCounter()
    {
        lastDateTime = default;
        timeLeft = 0;

        stopped = true;
    }

    public void SetTimeToComplete(float time)
    {
        timeToComplete = time;
    }

    public void StartCounter()
    {
        lastDateTime = DateTime.Now;
        stopped = false;
    }

    public void StopCounter()
    {
        stopped = true;
    }
    #endregion

    #region Private Methods
    private bool UpdateCounter()
    {
        SetTimeLeft();

        finished = timeLeft >= timeToComplete;

        if (finished)
            ResetCounter();

        return finished;
    }

    private float SetTimeLeft()
    {
        if (!stopped)
        {
            TimeSpan addSeconds = DateTime.Now - lastDateTime;
            timeLeft += (float)addSeconds.TotalSeconds;

            //Update lastDateTime
            lastDateTime = DateTime.Now;
        }

        return timeLeft;
    }

    private float GetTimeLeft()
    {
        float tl = 0;

        tl = timeToComplete - timeLeft;

        return tl;
    }
    #endregion
}
