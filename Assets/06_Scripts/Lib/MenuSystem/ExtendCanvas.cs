using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
public class ExtendCanvas : MonoBehaviour
{

    [SerializeField]
    bool m_isInteractable = true;

    Canvas m_canvas;
    Canvas Canvas
    {
        get
        {
            if(m_canvas == null)
            {
                m_canvas = GetComponent<Canvas>();
            }
            return m_canvas;
        }
    }

    public bool IsEnabled() => GraphicRaycaster.enabled;

    GraphicRaycaster m_graphicraycaster;
    GraphicRaycaster GraphicRaycaster
    {
        get
        {
            if (m_graphicraycaster == null)
            {
                m_graphicraycaster = GetComponent<GraphicRaycaster>();
            }
            return m_graphicraycaster;
        }
    }


    private void OnEnable()
    {
        InformChilds(true);
        SetGraphicRaycasterEnabled(true);
        Canvas.enabled = true;
    }


    private void OnDisable()
    {
        InformChilds(false);
        SetGraphicRaycasterEnabled(false);
        Canvas.enabled = false;
    }

    void InformChilds(bool a_enable)
    {
        ExtendCanvas[] canvas = GetComponentsInChildren<ExtendCanvas>(false);
        foreach (ExtendCanvas canva in canvas)
        {
            canva.ParentUpdate(a_enable);
        }
    }

    public void ParentUpdate(bool a_enable)
    {
        SetGraphicRaycasterEnabled(a_enable);
    }

    void SetGraphicRaycasterEnabled(bool a_enabled)
    {
        if (m_isInteractable)
        {
            GraphicRaycaster.enabled = a_enabled;
        }
        else
        {
            GraphicRaycaster.enabled = false;
        }
    }
}
