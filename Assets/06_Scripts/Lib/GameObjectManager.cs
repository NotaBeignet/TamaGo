using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum SPAWN_CONTAINER_TYPE { NOTHING, DESTRUCTIBLE, UNDESTRUCTIBLE , NBCATEGORY};

public class GameObjectManager : Singleton<GameObjectManager>
{
    Dictionary<SPAWN_CONTAINER_TYPE, GameObject> m_containers = new Dictionary<SPAWN_CONTAINER_TYPE, GameObject>();

    protected override void Awake()
    {
        base.Awake();
        for(int i = 0; i < (int)SPAWN_CONTAINER_TYPE.NBCATEGORY; ++i )
        {
            GameObject go = new GameObject("GameObjectManagerContainer : " + (SPAWN_CONTAINER_TYPE) i);
            m_containers.Add((SPAWN_CONTAINER_TYPE)i, go);
        }
    }

    public GameObject InstantiateObject(GameObject a_gameObject, Vector3 a_position, Quaternion a_rotation, SPAWN_CONTAINER_TYPE a_type = SPAWN_CONTAINER_TYPE.NOTHING)
    {
        Assert.AreNotEqual(a_type, SPAWN_CONTAINER_TYPE.NBCATEGORY, "Type not allowed");

        GameObject container = m_containers[a_type];

       // Debug.Log("[GameObjectManager] Instantiate :" + a_gameObject);

        return Instantiate(a_gameObject, a_position, a_rotation, container.transform);

    }
    
    public void DestroyObjects(SPAWN_CONTAINER_TYPE a_type)
    {

        Assert.AreNotEqual(a_type, SPAWN_CONTAINER_TYPE.NBCATEGORY, "Type not allowed");

        GameObject container = m_containers[a_type];

        Utils.DestroyChilds(container.transform);
    }
}
