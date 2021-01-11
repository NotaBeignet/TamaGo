using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TabMenu : MonoBehaviour
{
    [SerializeField]
    Tab m_tabPrefab;

    [SerializeField]
    Transform m_tabContainer;

    List<Tab> m_tabs = new List<Tab>();

    // Start is called before the first frame update
    void Start()
    {
        Fill();
    }

    public void Fill()
    {
        TabPage[] m_children = GetComponentsInChildren<TabPage>();

        foreach (TabPage tabPage in m_children)
        {
            Tab newTab = Instantiate(m_tabPrefab, m_tabContainer);
            newTab.Name = tabPage.Name;
            newTab.LinkedTabMenu = this;
            newTab.LinkedPage = tabPage;
            newTab.SetClose();
            m_tabs.Add(newTab);

            tabPage.Init(this);
        }

        OpenTab(0);
    }

    public void CloseAll()
    {
        foreach(Tab tab in m_tabs)
        {
            tab.SetClose();
        }
    }

    public void OpenTab(int a_index)
    {
        if(a_index >= 0 && a_index < m_tabs.Count)
        {
            m_tabs[a_index].Open();
        }
    }

    public Tab GetTab(int a_index)
    {
        if(a_index >= 0 && a_index < m_tabs.Count)
        {
            return m_tabs[a_index];
        }
        else
        {
            return null;
        }
    }

    public void Refresh()
    {
        foreach(Tab tab in m_tabs)
        {
            tab.Refresh();
        }
    }

}
