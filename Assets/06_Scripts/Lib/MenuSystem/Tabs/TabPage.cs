using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
public abstract class TabPage : MonoBehaviour
{
    [SerializeField]
    string m_name = "NoName";
 
    public string Name { get => m_name; private set => m_name = value; }

    bool m_isActive;
    public bool IsActive
    {
        get
        {
            return m_isActive;
        }

        set
        {
            m_isActive = value;
            GetComponent<Canvas>().enabled = m_isActive;
            if(IsActive)
            {
                OnOpen();
            }
        }
    }

    protected virtual void OnOpen() {}

    public virtual void Init(TabMenu a_tabMenu)
    {
        
    }
}
