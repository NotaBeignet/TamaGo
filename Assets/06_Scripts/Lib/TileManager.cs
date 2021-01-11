using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : Singleton<TileManager>
{
    const int m_maxX = 10;
    const int m_maxY = 70;
    Dictionary<int, bool[,]> m_spawned = new Dictionary<int, bool[,]>();
    Dictionary<int, int> m_count = new Dictionary<int, int>();

    int m_nextFreeId = 0;

    public bool CanSpawn(int a_id, int a_x, int a_y)
    {
        if (!m_spawned.ContainsKey(a_id))
        {
            return true;
        }

        return !m_spawned[a_id][((a_x % m_maxX) + m_maxX) % m_maxX, ((a_y % m_maxY) + m_maxY) % m_maxY];
    }

    public void Spawn(int a_id, int a_x, int a_y)
    {
        if (!m_spawned.ContainsKey(a_id))
        {
            m_spawned[a_id] = new bool[m_maxX, m_maxY];
            m_count[a_id] = 0;
        }
        ++m_count[a_id];
        m_spawned[a_id][((a_x % m_maxX) + m_maxX) % m_maxX, ((a_y % m_maxY) + m_maxY) % m_maxY] = true;

        if(m_nextFreeId == a_id)
        {
            ++m_nextFreeId;
            m_nextFreeId = GetNextFreeId();
        }

     //   Debug.Log("Count " + a_id + "  " + m_count[a_id]);

    }

    public void Free(int a_id, int a_x, int a_y)
    {
        if (!m_spawned.ContainsKey(a_id))
        {
            return;
        }
        --m_count[a_id];
        m_spawned[a_id][((a_x % m_maxX) + m_maxX) % m_maxX, ((a_y % m_maxY) + m_maxY) % m_maxY] = false;

        if(m_count[a_id] == 0)
        {
            if(m_nextFreeId > a_id)
            {
                m_nextFreeId = a_id;
            }

        }
       // Debug.Log("Count " + a_id + "  " + m_count[a_id]);

    }

    public int GetNextFreeId()
    {
        int index = m_nextFreeId;
        while (true)
        {
            if(!m_count.ContainsKey(index) || m_count[index] == 0)
            {
                return index;
            }
            ++index;
        }
    }

}
