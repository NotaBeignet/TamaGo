using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : ScriptableObject
{

    CHARACTERISTIC m_characteristic;
    float m_duration;
    float m_frequency;
    float m_intensity;
    Timer m_timer;

    public void Start()
    {
        if (m_timer == null)
        {
            m_timer = TimerFactory.Instance.GetTimer();
            List<Frequency> frequencies = new List<Frequency>
            {
                new Frequency(m_frequency, Apply)
            };
            m_timer.InitializeTimer(m_duration, Stop, frequencies);

        }
        m_timer.StartTimer();


    }

    void Apply()
    {
        GameManager.Instance.GetCharacteristic(m_characteristic)?.IncValue(m_intensity);
    }

    public void Stop()
    {
        // If stackable, we want to destroy the timer. 
        // Add events here to stop VFX ? 
        m_timer?.Stop();
    }
}
