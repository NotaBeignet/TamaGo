using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Frequency 
{
    float m_frequency;
    Action m_callback;
    //todo check if enough to avoid overflow
    int m_nextFrequency = 1;

    public Frequency(float frequency, Action callback)
    {
        m_frequency = frequency;
        m_callback = callback;
    }

    public void CheckFrequency(float a_currentTime)
    {
        if (a_currentTime >= m_frequency * m_nextFrequency)
        {
            ++m_frequency;
            m_callback?.Invoke();
        }
    }

    public void Initialize()
    {
        m_nextFrequency = 1;
    }
}

public class Timer : MonoBehaviour
{

    float m_finishTime = 1;
    bool m_running;
    float m_currentTime = 0;
    Action m_callback;
    List<Frequency> m_frequencies;

    bool m_isUnscale;

    Action<Timer> m_listeners;

    public bool IsUnscale { get => m_isUnscale; set => m_isUnscale = value; }
    public float FinishTime 
    { 
        get
        {
            return m_finishTime;
        }
        set
        {
            if (value < 0)
            {
                m_finishTime = Mathf.Infinity;
            }
            else
            {
                m_finishTime = value;
            }
        }
    }
    public Action Callback { get => m_callback; set => m_callback = value; }
    public Action<Timer> Listeners { get => m_listeners; set => m_listeners = value; }

    void Awake()
    {
		m_running = false;
        m_currentTime = 0;
    }

    public void InitializeTimer(float a_finishTime = Mathf.Infinity, Action a_callback = null, List<Frequency> a_frequencies = null )
    {
        FinishTime = a_finishTime;
        Callback = a_callback;
        m_frequencies = a_frequencies ?? new List<Frequency>();
    }

    public void StartTimer()
    {
        m_currentTime = 0;
        for (int i = 0; i < m_frequencies.Count; ++i)
        {
            m_frequencies[i].Initialize();
        }
        m_running = true;
    }

    public bool IsTimeUp()
    {
        return m_currentTime >= FinishTime;
	}

	void Update ()
    {
		if (!m_running)
        {
			return;
		}

		m_currentTime += (IsUnscale ? Time.unscaledDeltaTime : Time.deltaTime);

        for (int i = 0; i < m_frequencies.Count; ++i)
        {
            m_frequencies[i].CheckFrequency(m_currentTime);
        }

        if (IsTimeUp())
        {
            m_running = false;
            Callback?.Invoke();
        }

        if(Listeners != null)
        {
            Listeners.Invoke(this);
        }
    }

    public bool IsTimerRunning(){
		return m_running;
	}

    public void Stop()
    {
        m_running = false;
        m_currentTime = FinishTime;
    }
    public void Pause()
    {
        m_running = false;
    }

    public void UnPause()
    {
        if (!IsTimeUp())
        {
            m_running = true;
        }
    }

    public float GetCurrentTime()
    {
        return m_currentTime;
    }

    public float GetTimeLeft()
    {
        return FinishTime - m_currentTime;
    }

    public float GetLength()
    {
        return FinishTime;
    }

    public float GetPercent()
    {
        return FinishTime <= 0 ? 1 : m_currentTime / FinishTime;
    }

    public override string ToString()
    {
        float timeLeft = GetTimeLeft();
        string minLeft = ((int)timeLeft / 60).ToString();
        string secLeft = ((int)timeLeft % 60).ToString();

        return minLeft + ":" + secLeft;
    }


    public void Destroy()
    {
        Destroy(this);
    }
}
