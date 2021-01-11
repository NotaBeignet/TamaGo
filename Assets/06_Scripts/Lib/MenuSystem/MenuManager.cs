using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;

public enum MENUTYPE { NOTHING, MAIN, END, GAME, PAUSE, SCORE, CHRONO, LOADING, START, LEVEL_CHOOSER, WIN, OPTION , OPTION_CONTROLS, OPTION_GRAHICS, MAP, STATS, HIGHSCORE, TUTORIAL_END, MUSIC};


public class MenuManager : Singleton<MenuManager>
{
    [SerializeField]
    GameObject m_UI;

	List<Menu> m_listMenu;

	[SerializeField]
	MENUTYPE m_startMenu;

    Stack<MENUTYPE> m_history = new Stack<MENUTYPE>();

	void Start ()
    {
        m_listMenu = new List<Menu>(m_UI.GetComponentsInChildren<Menu>());
        ClearHistory();
        OpenMenu(m_startMenu);
    }


    public void ClearHistory()
    {
        m_history.Clear();
        m_history.Push(MENUTYPE.NOTHING);
        foreach (Menu menu in m_listMenu)
        {
            RealClose(menu, null);
        }
    }


    void Update () 
    {
        MENUTYPE currentMenuType = m_history.Peek();
        if(currentMenuType != MENUTYPE.NOTHING)
        {
            Menu menu = GetMenu(m_history.Peek());
            if (menu != null)
            {
                MenuBackGround.Instance.SetAlpha(menu.GetAlphaBack());
            }
        }
        else
        {
            MenuBackGround.Instance.Disable();
        }

      /*  if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
        {
            if(m_history.Count > 2)
            {
                Back();
            }
            else
            {
                Utils.QuitApp();
            }
        }*/
    }

    public void OpenMenu(MENUTYPE a_type, MENUTYPE a_previous = MENUTYPE.NOTHING)
	{
        if(a_type == MENUTYPE.NOTHING)
        {
            return;
        }
        
        if ( a_type != m_history.Peek())
		{
            CloseMenu(false, a_type);
        }
    }

    void RealOpen(MENUTYPE a_type, MENUTYPE a_previous = MENUTYPE.NOTHING)
    {
        Menu menu = GetMenu(a_type);
        if (menu != null)
        {
            EnableMenu(menu, true);

            if (a_previous == MENUTYPE.NOTHING)
            {
                a_previous = m_history.Peek();
            }
            if(m_history.Peek() != a_type)
            {
                m_history.Push(a_type);
            }
            menu.OnOpen(a_previous);
        }
    }


    public void CloseMenu(bool a_isBack = false, MENUTYPE a_nextMenu = MENUTYPE.NOTHING)
	{
        CloseMenu(m_history.Peek(), a_isBack, a_nextMenu);
    }


    public void CloseMenu(MENUTYPE a_menuType, bool a_isBack = false, MENUTYPE a_nextMenu = MENUTYPE.NOTHING)
    {
        Menu menu = GetMenu(a_menuType);
        CloseMenu(menu, a_isBack, a_nextMenu);
    }


    public void CloseMenu(Menu a_menu, bool a_isBack = false, MENUTYPE a_nextMenu = MENUTYPE.NOTHING)
    {
        if (a_menu != null)
        {
            //if it'sthe current menu, process
            if(m_history.Peek() == a_menu.MenuType)
            {
                Action callback;
                MENUTYPE previous = m_history.Peek();
                if (a_isBack)
                {
                    m_history.Pop();
                    a_nextMenu = m_history.Peek();
                }
                callback = () => RealOpen(a_nextMenu, previous);
                a_menu.OnClose(a_nextMenu, () => RealClose(a_menu, callback));
            }
        }
        else
        {
            RealOpen(a_nextMenu);
        }
    }

    public void Back()
    {
        CloseMenu(true);
    }

    void RealClose(Menu a_menu, Action a_callback)
    {
        EnableMenu(a_menu, false);
        if (a_callback != null)
        {
            a_callback();
        }
    }

    void EnableMenu(Menu a_menu, bool a_enable)
    {
        a_menu.GetComponent<ExtendCanvas>().enabled = a_enable;
    }


    public void BackToMainMenu()
    {
        CloseAll();
        OpenMenu(m_startMenu);
    }

    public void CloseAll()
    {
        CloseMenu();
        ClearHistory();
    }

    Menu GetMenu(MENUTYPE a_menuType)
    {
        return m_listMenu.Find(x => x.MenuType == a_menuType);
    }

}