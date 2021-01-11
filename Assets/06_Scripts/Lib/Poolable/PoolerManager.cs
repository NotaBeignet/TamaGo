using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CommonPoolableType { BurstRenderTarget }

[System.Serializable]
public class CommonPoolable
{

    [SerializeField]
    CommonPoolableType m_type;

    [SerializeField]
    Poolable m_prefab;

    [SerializeField]
    int m_initialPool;

    public CommonPoolableType Type { get => m_type; set => m_type = value; }
    public Poolable Prefab { get => m_prefab; set => m_prefab = value; }
    public int InitialPool { get => m_initialPool; set => m_initialPool = value; }
}


public class PoolerManager : Singleton<PoolerManager>
{

    [SerializeField]
    List<CommonPoolable> m_commonPoolable;

    [SerializeField]
    float m_checkInterval = 3.5f;

    [SerializeField]
    float m_timeOutPoolable = 5;


    Dictionary<int, List<Poolable>> m_spawned = new Dictionary<int, List<Poolable>>();
    Dictionary<int, Stack<Poolable>> m_destroyed = new Dictionary<int, Stack<Poolable>>();

    System.Predicate<Poolable> m_predicate;

    Dictionary<int, Stack<Poolable>>.Enumerator m_enumerator;


#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField]
    bool m_debugCount;

    [SerializeField]
    bool m_debugClean;


    [SerializeField]
    bool m_debugDoubleDestroy;
#endif


    private void Start()
    {
        m_predicate = (o) => o.IsCleanable && o.TimeOutTime + m_timeOutPoolable > Time.time;
        m_enumerator = m_destroyed.GetEnumerator();
        EventManager.Instance.RegisterOnClean((o) =>
        {
            foreach(CommonPoolable common in m_commonPoolable)
            {
                PoolerManager.Instance.SetDirty(common.Prefab);
            }
        });

        foreach (CommonPoolable common in m_commonPoolable)
        {
            for(int i = 0; i < common.InitialPool; ++i)
            {
                SetDestroyed(InstantiatePoolable(common.Prefab));
            }
        }

        //  StartCoroutine(CheckDestroyable());
    }


    public T InstantiatePoolable<T>(T a_go, Vector3? a_position = null) where T : Poolable
    {
        T res = null;

        int id = a_go.GetInstanceID();

        List<Poolable> list = null;

        if (!m_spawned.ContainsKey(id))
        {
            //  Debug.Log("Pooler don't have " + a_go.name);
            list = new List<Poolable>();
            m_spawned[id] = list;
            m_destroyed[id] = new Stack<Poolable>();
            ResetEnumerator();
        }
        else
        {
            list = m_spawned[id];
            if (m_destroyed[id].Count > 0)
            {
                res = (T)(m_destroyed[id].Pop());
            }
        }

        if (res == null)
        {
            res = GameObject.Instantiate(a_go);
            res.PrefabId = id;
            list.Add(res);
        }


#if UNITY_EDITOR
        if (m_debugCount)
        {
            Debug.Log(a_go.name + " Spawned Count " + list.Count);
            Debug.Log(a_go.name + " Destroyed Count " + m_destroyed[id].Count);
        }
#endif

        res.transform.position = a_position ?? new Vector3(1000, 1000, 0);
        return res;
    }

    public T GetCommonPoolableInstance<T>(CommonPoolableType a_type, Vector3? a_position = null) where T : Poolable
    {
        return InstantiatePoolable<T>((T)m_commonPoolable.Find((o) => o.Type == a_type).Prefab, a_position);
    }


    IEnumerator CheckDestroyable()
    {
        bool checkable = false;
        while (true)
        {
            yield return UtilsYield.GetWaitForSeconds(m_checkInterval);

            if (checkable)
            {
                Poolable[] tocheck = m_enumerator.Current.Value.ToArray();
                m_enumerator.Current.Value.Clear();
                for (int i = 0; i < tocheck.Length; ++i)
                {
                    Poolable value = tocheck[i];
                    if (m_predicate(value))
                    {
#if UNITY_EDITOR
                        if (m_debugClean)
                        {
                            Debug.Log("Cleaning " + value.name + "   " + value.transform.position);
                        }
#endif
                        m_spawned[value.PrefabId].Remove(value);
                        Destroy(value.gameObject);
                    }
                    else
                    {
                        m_enumerator.Current.Value.Push(value);
                    }
                }
            }
            else
            {
                m_enumerator.Dispose();
                m_enumerator = m_destroyed.GetEnumerator();
            }


            checkable = m_enumerator.MoveNext();

        }
    }


    public void SetDirty(Poolable a_poolablePrefab)
    {
        if (m_spawned.ContainsKey(a_poolablePrefab.GetInstanceID()))
        {
            m_spawned[a_poolablePrefab.GetInstanceID()].ForEach((o) => o.IsDestroyed = true);
        }
    }


    public void SetDestroyed(Poolable a_poolable)
    {
        if (m_spawned.ContainsKey(a_poolable.PrefabId))
        {
#if UNITY_EDITOR
            if (m_debugDoubleDestroy && m_destroyed[a_poolable.PrefabId].Contains(a_poolable))
            {
                Debug.LogError(a_poolable + " have been set destroyed two times");
            }
#endif
            m_destroyed[a_poolable.PrefabId].Push(a_poolable);
        }
    }

    void ResetEnumerator()
    {
        m_enumerator.Dispose();
        m_enumerator = m_destroyed.GetEnumerator();
        m_enumerator.MoveNext();
    }
}