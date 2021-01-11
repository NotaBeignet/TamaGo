using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ExtendButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    System.Action m_callbackOnHold;

    public Action CallbackOnHold { get => m_callbackOnHold; set => m_callbackOnHold = value; }

    bool m_isPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        m_isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isPressed = false;
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isPressed && CallbackOnHold != null)
        {
            CallbackOnHold();
        }
    }
}
