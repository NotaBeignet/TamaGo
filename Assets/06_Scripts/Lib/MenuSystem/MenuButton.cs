using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField]
    MENUTYPE m_menuToOpen;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => Click());        
    }


    void Click()
    {
        MenuManager.Instance.OpenMenu(m_menuToOpen);
    }
}
