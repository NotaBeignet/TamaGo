using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tab : MonoBehaviour, IPointerClickHandler
{

    string m_name = "NoName";
    TabPage m_linkedPage;
    TabMenu m_linkedTabMenu;

    public string Name
    {
        private get
        {
            return m_name;
        }


        set
        {
        
            m_name = value;
            m_text.text = m_name;
        }
    }
    public TabPage LinkedPage { get => m_linkedPage; set => m_linkedPage = value; }
    public TabMenu LinkedTabMenu { get => m_linkedTabMenu; set => m_linkedTabMenu = value; }



    protected Image Background
    {
        get
        {
            if(m_background == null)
            {
                m_background = GetComponent<Image>();
            }

            return m_background;
        }


        set
        {

            m_background = value;
        }



    }


    [SerializeField]
    Text m_text;


    [SerializeField]
    Color m_enable;

    [SerializeField]
    Color m_disable;


    Image m_background;



    public void OnPointerClick(PointerEventData eventData)
    {
        Open();
    }


    public void SetOpen()
    {
        m_linkedPage.IsActive = true;
        Background.color = m_enable;
    }

    public void SetClose()
    {
        m_linkedPage.IsActive = false;
        Background.color = m_disable;
    }


    public void Open()
    {
        if (!CanOpen())
            return;

        m_linkedTabMenu.CloseAll();
        SetOpen();
    }

    protected virtual bool CanOpen()
    {
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {

        Background = GetComponent<Image>();
    }

    public virtual void Refresh() { }
}
