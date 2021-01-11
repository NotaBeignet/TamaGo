using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableUseless : Interactable
{

    [SerializeField]
    GameObject m_pannel;

    protected override void Start()
    {
        base.Start();
        m_isToggleUse = true;
    }

    protected override void Interact(PlayerController a_player)
    {
        m_pannel.SetActive(true);

        base.Interact(a_player);
    }

    public void SetPlayerColor(string a_color)
    {
        if (IsUsing)
        {
            Color color = Utils.ParseColor(a_color);
            Renderer renderer = m_user.GetComponent<Renderer>();
            renderer.material.SetColor("_BaseColor", color);
        }
    }

    public void Close()
    {
        Release();
    }

    protected override void Release()
    {
        base.Release();
        m_pannel.SetActive(false);

    }


}
