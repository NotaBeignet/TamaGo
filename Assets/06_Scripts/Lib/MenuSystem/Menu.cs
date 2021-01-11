using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(ExtendCanvas))]
[RequireComponent(typeof(GraphicRaycaster))]
public abstract class Menu : MonoBehaviour
{

    bool m_isOpen;

    public bool IsOpen { get => m_isOpen; set => m_isOpen = value; }
    public MENUTYPE MenuType { get => m_menuType; set => m_menuType = value; }

    protected float m_alphaBack = 1.0f;

    [SerializeField]
    MENUTYPE m_menuType;

    protected virtual void Start()
    {
        if(m_menuType == MENUTYPE.NOTHING)
        {
            Debug.LogError(this + " menutype is set to nothing");
        }
    }

    public virtual void OnOpen(MENUTYPE a_previousMenu)
    {
        IsOpen = true;
    }

    public virtual void OnClose(MENUTYPE a_nextMenu, Action a_callback)
    {
        IsOpen = false;
        a_callback();
    }

    public virtual float GetAlphaBack()
    {
        return m_alphaBack;
    }
}