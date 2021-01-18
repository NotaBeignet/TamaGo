using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Buff", order = 2)]
public class Buff : ScriptableObject
{
    [SerializeField]
    CHARACTERISTIC m_characteristic;

    [SerializeField]
    float m_duration;

    [SerializeField]
    float m_frequency;

    [SerializeField]
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
