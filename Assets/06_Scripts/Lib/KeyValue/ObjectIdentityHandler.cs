using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectIdentityHandler : MonoBehaviour
{

    [SerializeField]
    ObjectIdentity m_objectIdentity;

    public ObjectIdentity ObjectIdentity { get => m_objectIdentity; set => m_objectIdentity = value; }

    public void Awake()
    {
        if(m_objectIdentity == null)
        {
            m_objectIdentity = new ObjectIdentity();
        }
    }

    public string GetName()
    {
        return m_objectIdentity.name;
    }
}
