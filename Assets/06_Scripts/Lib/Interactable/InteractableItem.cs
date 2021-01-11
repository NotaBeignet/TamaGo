using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItem : Interactable
{
    /*[SerializeField]
    Item m_item;*/

    [SerializeField]
    bool m_multiple;


    bool m_isUsed;


    protected override void Start()
    {
        base.Start();
        if (m_identity.ObjectIdentity.IsTaken())
        {
            Taken();
        }
    }


    public override bool IsInteractable(PlayerController a_player)
    {
        return base.IsInteractable(a_player) && !m_isUsed;
    }


    protected override void Interact(PlayerController a_player)
    {
        base.Interact(a_player);

     /*   a_player.Inventory.AddItem(m_item);*/

        Taken();
        
        /*a_player.ResetInteractable();*/
    }

    void Taken()
    {
        if (!m_multiple)
        {
            m_isUsed = true;
            m_identity.ObjectIdentity.Taken(true);
            Destroy(gameObject);
        }
    }
}
