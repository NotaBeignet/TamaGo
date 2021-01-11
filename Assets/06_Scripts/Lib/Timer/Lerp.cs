using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Lerp<T> : MonoBehaviour
{

    Action<T> m_callback;

    Timer m_timer;

    private float minValue;
    private float maxValue;

    bool m_isRunning;

    public float MinValue { get => minValue; set => minValue = value; }
    public float MaxValue { get => maxValue; set => maxValue = value; }


    // Start is called before the first frame update
    void Start()
    {
        CreateTimer();
    }


    void CreateTimer()
    {
        if (!m_timer)
        {
            m_timer = TimerFactory.Instance.GetTimer();
        }
        m_timer.Callback = () =>
        {
            TriggerCallback();
            m_isRunning = false;
        };
    }

    public void Init(bool a_isUnscaled, float a_minValue, float a_maxValue, float a_time, Action<T> a_callback)
    {
        CreateTimer();
        if (a_isUnscaled)
        {
            m_timer.IsUnscale = true;
        }
        m_timer.FinishTime = a_time;
        MinValue = a_minValue;
        MaxValue = a_maxValue;
        m_callback = a_callback;
    }

    public void SetTime(float a_time)
    {
        m_timer.FinishTime = a_time;
    }

    private void Update()
    {
        if (m_isRunning)
        {
            TriggerCallback();
        }
    }

    private void TriggerCallback()
    {
        m_callback(LerpValue(m_timer.GetPercent()));
    }

    public void StartLerp()
    {
        m_callback(LerpValue(0));
        m_isRunning = true;
        m_timer.RestartTimer();
    }

    public void Stop()
    {
        m_isRunning = false;
        m_timer.Stop();
        if(m_callback != null)
        {
            m_callback(LerpValue(1));
        }
    }

    protected abstract T LerpValue(float a_percent);
}


public class LerpFloat : Lerp<float>
{
    protected override float LerpValue(float a_percent)
    {
        return Mathf.Lerp(MinValue, MaxValue, a_percent);
    }
}

public class LerpFloatCurve : Lerp<float>
{
    AnimationCurve m_curve;

    public void Init(bool a_isUnscaled, float a_minValue, float a_maxValue, float a_time, Action<float> a_callback, AnimationCurve a_curve)
    {
        base.Init(a_isUnscaled, a_minValue, a_maxValue, a_time, a_callback);
        m_curve = a_curve;
    }

    protected override float LerpValue(float a_percent)
    {
        //not a lerp to handle < 0 & > 1
        return MinValue +  (MaxValue - MinValue) * m_curve.Evaluate(a_percent);
    }
}
