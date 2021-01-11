using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer : MonoBehaviour
{

    float m_finishTime = 1;
    bool m_running;
    float m_currentTime = 0;
    Action m_callback;

    bool m_isUnscale;

    Action<Timer> m_listeners;

    public bool IsUnscale { get => m_isUnscale; set => m_isUnscale = value; }
    public float FinishTime { get => m_finishTime; set => m_finishTime = value; }
    public Action Callback { get => m_callback; set => m_callback = value; }
    public Action<Timer> Listeners { get => m_listeners; set => m_listeners = value; }

    void Awake()
    {
		m_running = false;
        m_currentTime = 0;
    }

    public void StartTimer(float a_finishTime = Mathf.Infinity, Action a_callback = null  )
    {
        m_currentTime = 0;
        FinishTime = a_finishTime;
        Callback = a_callback;
        m_running = true;
    }

    public void RestartTimer()
    {
        m_currentTime = 0;
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
        if (IsTimeUp())
        {
            m_running = false;
            if (Callback != null)
            {
                Callback();
            }
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
