using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CHARACTERISTIC { HEALTH, HYGIENE, ALIGNEMENT };


[CreateAssetMenu(fileName = "Characteristic", menuName = "Characteristic", order = 1)]
public class CharacteristicsScriptable : ScriptableObject
{
    public float m_min;
    public float m_max;
    public float m_current;

    public void SetValue(float a_value)
    {
        m_current = Mathf.Clamp(a_value, m_min, m_max);
        EventManager.Instance.InvokeOnCharacteristicUpdated(this);
    }
}
