using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDependantPoolable : Poolable
{

    const int m_defaultOffset = 20;

    protected float m_offsetHeightDestroy;

    public override void Awake()
    {
        base.Awake();
        StartCoroutine(CheckCameraPosition());
    }

    public void SetCameraOffsetDestroy(float a_offset)
    {
        if(a_offset < 0)
        {
            a_offset = m_defaultOffset;
        }

        m_offsetHeightDestroy = a_offset;
        IsDestroyed = false;
    }

    public void SetCameraOffsetDestroy()
    {
        SetCameraOffsetDestroy(m_defaultOffset);
    }


    IEnumerator CheckCameraPosition()
    {
        YieldInstruction yielInstruction = UtilsYield.GetWaitForSeconds(0.35f);
        yield return yielInstruction;
        while (true)
        {
            if (IsDestructible() && CameraManager.Instance.IsYPassed(GetPos() - new Vector3(0, m_offsetHeightDestroy, 0)))
            {
                IsDestroyed = true;
            }
            yield return yielInstruction;
        }
    }

    protected virtual bool IsDestructible()
    {
        return !IsDestroyed;
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();
    }

    protected override void OnRestore()
    {
        base.OnRestore();
    }

    protected virtual Vector3 GetPos()
    {
        return Transform.position;
    }
}