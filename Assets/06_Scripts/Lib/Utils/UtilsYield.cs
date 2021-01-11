using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//code from https://forum.unity.com/threads/c-coroutine-waitforseconds-garbage-collection-tip.224878/#post-2151707

public static class UtilsYield
{

    static Dictionary<float, WaitForSeconds> m_yields = new Dictionary<float, WaitForSeconds>();

    public static WaitForEndOfFrame EndOfFrame { get; } = new WaitForEndOfFrame();
    public static WaitForFixedUpdate FixedUpdate { get; } = new WaitForFixedUpdate();

    public static YieldInstruction GetWaitForSeconds(float a_seconds)
    {
        if (!m_yields.ContainsKey(a_seconds))
        {
            m_yields[a_seconds] = new WaitForSeconds(a_seconds);
        }

        return m_yields[a_seconds];
    }
}
