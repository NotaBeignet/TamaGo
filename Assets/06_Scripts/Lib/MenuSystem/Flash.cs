using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flash : Singleton<Flash>
{

    Image m_image;
    Timer m_timer;

    [SerializeField]
    float m_time = 0.1f;
    // Start is called before the first frame update
    protected override void Awake()
    {
        m_image = GetComponent<Image>();
        m_image.enabled = false;
        m_timer = TimerFactory.Instance.GetTimer();
        m_timer.FinishTime = m_time;
        m_timer.Callback = () => m_image.enabled = false;
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerFlash(Color? a_color = null)
    {
        m_image.color = a_color ?? Color.white;
        m_image.enabled = true;
        m_timer.RestartTimer();
    }

}
