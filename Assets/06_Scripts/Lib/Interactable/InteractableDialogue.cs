using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDialogue : Interactable
{
    [SerializeField]
    [Tooltip("The name of the xml dialogue file in the Resources folder.")]
    private string m_dialoguePath = "";

    [SerializeField]
    [Tooltip("True if the dialogue can be replayed. Else can only be played once. - NOT IMPLEMENTED")]
    private bool m_multiplePass = true;

    protected override void Start()
    {
        base.Start();
        m_isToggleUse = true;
    }


    protected override void ApplyUpdate()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (DialogueManager.Instance.ContinueDialogue())
            {
                Release();
            }
        }
    }

    protected override void Interact(PlayerController a_player)
    {
        base.Interact(a_player);

        DialogueManager.Instance.ReadDialogue(m_dialoguePath);
        SpokeTo();
    }

    private void SpokeTo()
    {
        m_identity.ObjectIdentity.SpokeTo(true);
    }

    protected override void Release()
    {
        base.Release();
        DialogueManager.Instance.Close();
    }
}
