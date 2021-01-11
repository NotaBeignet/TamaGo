using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollSnapItem : MonoBehaviour, IPointerClickHandler
{
    ScrollSnap m_parentScrollSnap;

    [SerializeField]
    RectTransform m_container;

    [SerializeField]
    GameObject m_greyFilter;

    [SerializeField]
    Vector3 m_scaleUnselected;

    [SerializeField]
    float m_timeLerpScale = 1;

    [SerializeField]
    float m_bouncinessLerpScale = 1;


    bool m_isLerping;

    protected bool m_isCurrent = true;

    float m_timeLerping;

    public void Awake()
    {
        SetIsSelected(false);
        m_parentScrollSnap = GetComponentInParent<ScrollSnap>();
    }

    public void Update()
    {
        if (m_isLerping)
        {
            m_timeLerping += Time.deltaTime;
            float current = m_timeLerping / m_timeLerpScale;
            m_container.localScale = Utils.SpringDamper(m_isCurrent ? m_scaleUnselected : Vector3.one, m_isCurrent ? Vector3.one : m_scaleUnselected, current, m_bouncinessLerpScale);
            m_isLerping = current < 1;
        }
    }

    public void SetIsSelected(bool a_value)
    {
        if(m_isCurrent != a_value)
        {
            m_isCurrent = a_value;
            m_isLerping = true;
            m_timeLerping = 0;
            m_greyFilter.SetActive(!a_value);
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if(!m_isCurrent)
        {
            m_parentScrollSnap.ChangePage(transform.GetSiblingIndex());
        }
    }
}
