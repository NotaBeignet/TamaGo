using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : Singleton<WindowManager>
{
    float m_lastDPIFactor;

    protected override void Awake()
    {
        base.Awake();
        m_lastDPIFactor = 1;
    }

    void FixedUpdate()
    {
        float dpiFactor = QualitySettings.resolutionScalingFixedDPIFactor;
        if (m_lastDPIFactor != dpiFactor)
        {
            m_lastDPIFactor = dpiFactor;
            EventManager.Instance.InvokeOnResolutionUpdated(this);
        }
    }
}
