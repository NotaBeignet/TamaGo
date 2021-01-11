using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCycle
{

    int m_currentIndex;
    int m_cycleThreshold;

    int[] m_randomArray;

    public int Count 
    {
        get
        {
            return m_randomArray.Length;
        }
    }

    public RandomCycle(int[] a_values, int a_cyleThreshold = -1)
    {
        if (a_values.Length == 0)
        {
            Debug.LogError("Random Cycle init with a size of 0");
        }

        if (a_cyleThreshold < 0)
        {
            a_cyleThreshold = a_values.Length;
        }

        m_randomArray = a_values;

        m_cycleThreshold = a_cyleThreshold;

        ResetRandomList();
    }

    public RandomCycle(int a_size, int a_cyleThreshold = -1)
    {
        if(a_size == 0)
        {
            Debug.LogError("Random Cycle init with a size of 0");
        }

        if(a_cyleThreshold < 0)
        {
            a_cyleThreshold = a_size;
        }

        m_randomArray = new int[a_size];

        m_cycleThreshold = a_cyleThreshold;
        for (int i = 0; i < a_size; ++i)
        {
            // m_randomList.Add(i);
            m_randomArray[i] = i;
        }
        ResetRandomList();

    }

    // in order to keep the same value when we reset to avoid to have [1, 2, 3, reset, 3]
    //TODO : maybe I should randomize where I put the last element because if I cycle on the size of the thing, the last element will always be last
    void ResetRandomList()
    {
        int overrideCount = m_randomArray.Length - 1;
        if (m_currentIndex > 0 && m_randomArray.Length > 1)
        {
            int temp = m_randomArray[m_randomArray.Length - 1];
            m_randomArray[m_randomArray.Length - 1] = m_randomArray[m_currentIndex - 1];
            m_randomArray[m_currentIndex - 1] = temp;
        }

        Utils.ShuffleArray(m_randomArray, overrideCount);
        m_currentIndex = 0;
    }

    public int GetNext()
    {
        if(m_currentIndex >= m_cycleThreshold || m_currentIndex >= m_randomArray.Length)
        {
            ResetRandomList();
        }

        int res = m_randomArray[m_currentIndex];

        ++m_currentIndex;

        return res;
    }

}
