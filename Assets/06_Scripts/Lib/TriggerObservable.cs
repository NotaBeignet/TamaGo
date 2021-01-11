using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TriggerObservable : MonoBehaviour
{

    List<Action<TriggerObservable, Collider>> m_onEnterActions = new List<Action<TriggerObservable, Collider>>();
    List<Action<TriggerObservable, Collider>> m_onStayActions = new List<Action<TriggerObservable, Collider>>();
    List<Action<TriggerObservable, Collider>> m_onExitActions = new List<Action<TriggerObservable, Collider>>();

    List<Collider> m_colliderInside = new List<Collider>();


    public void Register(Action<TriggerObservable, Collider> a_callbackOnEnter, Action<TriggerObservable, Collider> a_callbackOnStay, Action<TriggerObservable, Collider> a_callbackOnExit)
    {
        if (a_callbackOnEnter != null)
        {
            m_onEnterActions.Add(a_callbackOnEnter);
        }
        if (a_callbackOnStay != null)
        {
            m_onStayActions.Add(a_callbackOnStay);
        }
        if (a_callbackOnExit != null)
        {
            m_onExitActions.Add(a_callbackOnExit);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        m_colliderInside.Add(other);
        foreach (Action<TriggerObservable, Collider> action in m_onEnterActions)
        {
            action(this, other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        foreach (Action<TriggerObservable, Collider> action in m_onStayActions)
        {
            action(this, other);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        m_colliderInside.Remove(other);
        foreach (Action<TriggerObservable, Collider> action in m_onExitActions)
        {
            action(this, other);
        }
    }

    public bool IsInside(string a_tag)
    {
        return (m_colliderInside.Find((o) => o.CompareTag(a_tag)) != null);
    }
}
