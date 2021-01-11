using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FPSDisplay : MonoBehaviour, IPointerDownHandler
{
    Text m_text;

    int m_lastFrameCount = 0;

    [SerializeField]
    Canvas m_canvas;

    private void Start()
    {
        m_text = GetComponentInChildren<Text>();
        StartCoroutine(CountFrame());
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        m_canvas.enabled = !m_canvas.enabled;
    }



    IEnumerator CountFrame()
    {
        while (true)
        {
            yield return UtilsYield.GetWaitForSeconds(1.0f);
            if (m_canvas.enabled)
            {
                m_text.text = "" + (Time.frameCount - m_lastFrameCount);
            }
            m_lastFrameCount = Time.frameCount;
        }
    }

}
