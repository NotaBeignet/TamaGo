using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSit : Interactable
{
    protected override void Start()
    {
        base.Start();
        m_isToggleUse = true;
    }

    protected override void Interact(PlayerController a_player)
    {
        base.Interact(a_player);
        CameraManager.Instance.CurrentStrategy = new CameraFPS(transform.position + new Vector3(0,0.5f,0));
    }

    protected override void Release()
    {
        base.Release();
        CameraManager.Instance.UnStack();
    }

}
