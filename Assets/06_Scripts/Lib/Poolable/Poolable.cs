using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Poolable : ExtendMonobehaviour
{

    public int PrefabId { get; set; }
    private bool m_isDestroyed = true;
    private bool m_isCleanable;

    protected Vector3 m_initScale;

    public System.Action OnDestroyedCallback;
    public virtual void Awake()
    {
        m_initScale = transform.localScale;
    }

    public bool IsDestroyed
    {
        get
        {
            return m_isDestroyed;
        }


        set
        {
            if (m_isDestroyed != value)
            {
                m_isDestroyed = value;
                if (m_isDestroyed)
                {
                    TimeOutTime = Time.time;
                    OnDestroyed();
                }
                else
                {
                    OnRestore();
                }
            }
        }
    }

    private float m_timeOutTime;

    public float TimeOutTime { get => m_timeOutTime; private set => m_timeOutTime = value; }
    public bool IsCleanable { get => m_isCleanable; set => m_isCleanable = value; }

    protected virtual void OnDestroyed()
    {
        if(OnDestroyedCallback != null)
        {
            OnDestroyedCallback();
            OnDestroyedCallback = null;
        }

        PoolerManager.Instance.SetDestroyed(this);
        Transform.localScale = m_initScale;
        Transform.position = new Vector3(1000, 1000, 0);
    }

    protected virtual void OnRestore(){}
}