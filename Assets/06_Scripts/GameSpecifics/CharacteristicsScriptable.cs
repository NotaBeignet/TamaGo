using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CHARACTERISTIC { HEALTH, HYGIENE, ALIGNEMENT };


[CreateAssetMenu(fileName = "Characteristic", menuName = "Characteristic", order = 1)]
public class CharacteristicsScriptable : ScriptableObject
{
    [SerializeField]
    CHARACTERISTIC m_name;

    float m_min;
    float m_max;
    float m_current;

    [SerializeField]
    float m_minInit;

    [SerializeField]
    float m_maxInit;

    [SerializeField]
    float m_currentInit;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField]
    bool m_debugLog;
#endif

    public float Current 
    { 
        
        get
        {
            return m_current;
        }
        set 
        {
            m_current = Mathf.Clamp(value, m_min, m_max);
            EventManager.Instance.InvokeOnCharacteristicUpdated(this);
#if UNITY_EDITOR
            if (m_debugLog)
            {
                Debug.Log("Charac " + m_name + " = " + m_current + " / " + m_max);
            }

#endif
        }

    }

    public CHARACTERISTIC Name { get => m_name; }

    public void IncValue(float a_value)
    {
        Current += a_value;
    }

    public void Initialize()
    {
        m_min = m_minInit;
        m_max = m_maxInit;
        Current = m_currentInit;
    }
    
}
