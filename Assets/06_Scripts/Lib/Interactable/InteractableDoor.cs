using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : Interactable
{

    [SerializeField]
    bool m_isOpen;

    [SerializeField]
    bool m_isLock;
  /*  [SerializeField]
    Item m_key;*/



    protected override void Start()
    {
        base.Start();
        if (m_identity.ObjectIdentity.HasLockInfos() && !m_identity.ObjectIdentity.IsLock())
        {
            m_isLock = false;
        }

        if (m_isLock)
        {
            m_isOpen = false;
            m_InteractableUI.SetText("E Unlock");
        }

        if (m_identity.ObjectIdentity.HasOpenInfos() && m_identity.ObjectIdentity.IsOpen())
        {
            Open();
        }

        m_identity.ObjectIdentity.Open(m_isOpen);
        m_identity.ObjectIdentity.Lock(m_isLock);
    }

    public override bool IsInteractable(PlayerController a_player)
    {
        return base.IsInteractable(a_player) && ((/*a_player.Inventory.HasItem(m_key) &&*/ m_isLock) || !m_isLock);
    }

    protected override void Interact(PlayerController a_player)
    {
        base.Interact(a_player);

        if (m_isLock)
        {
          /*  a_player.Inventory.UseItem(m_key);*/
            m_identity.ObjectIdentity.Lock(false);

            m_isLock = false;
            DebugPrint("Unlock");
        }

        ChangeState();

      /*  a_player.ResetInteractable();*/
    }

    public void ChangeState()
    {
        if (!m_isLock)
        {
            if (m_isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }

    void Open()
    {
        DebugPrint("Open");
        transform.position = new Vector3(transform.position.x + 5, transform.position.y, transform.position.z);
        m_identity.ObjectIdentity.Open(true);
        m_InteractableUI.SetText("E Close");

        m_isOpen = true;
    }

    void Close()
    {
        DebugPrint("Close");
        transform.position = new Vector3(transform.position.x - 5, transform.position.y, transform.position.z);
        m_identity.ObjectIdentity.Open(false);
        m_InteractableUI.SetText("E Open");

        m_isOpen = false;
    }


}
