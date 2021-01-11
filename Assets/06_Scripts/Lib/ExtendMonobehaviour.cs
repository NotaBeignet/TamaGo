using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendMonobehaviour : MonoBehaviour
{
    Transform m_transform;
    public Transform Transform
    {
        get
        {
            if (m_transform == null)
            {
                m_transform = transform;
            }
            return m_transform;
        }
    }
}
