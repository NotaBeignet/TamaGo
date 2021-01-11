using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableManager))]
[RequireComponent(typeof(ObjectIdentityHandler))]
public abstract class Interactable : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField]
    protected string m_debugName = "";


    [Space(20)]
    [Header("Variables")]
    [SerializeField]
    float m_coneAngle = 180.0f;

    bool m_playerInside = false;

    [SerializeField, Tooltip("higher is better")]
    int m_priotity = 0;

    [SerializeField]
    Condition m_condition;

    [SerializeField]
    protected InteractableUI m_InteractableUIPrefab;

    [SerializeField]
    Vector3 m_UIOffset = new Vector3(0, 3, 0);

    //Does this object need a press to exit the state
    [SerializeField]
    protected bool m_isToggleUse = false;
    protected PlayerController m_user = null;

    protected InteractableUI m_InteractableUI;


    protected ObjectIdentityHandler m_identity;


    public int Priotity { get => m_priotity; set => m_priotity = value; }
    public bool IsUsing { get; set; } = false;

    public virtual void OnEnter(PlayerController a_player)
    {
        m_playerInside = true;
        m_InteractableUI.Display(true);
        DebugPrint("PLayer Enter");
    }

    public virtual void OnExit(PlayerController a_player)
    {
        m_playerInside = false;
        m_InteractableUI.Display(false);
        DebugPrint("PLayer Exit");
    }

    public virtual void OnStay(PlayerController a_player)
    {
        //Debug.Log("PLayer Stay");
    }

    public virtual bool IsInteractable(PlayerController a_player)
    {

        Vector3 vecToMe = gameObject.transform.position - a_player.transform.position;
        float angle = Vector2.Angle(new Vector2(a_player.transform.forward.x, a_player.transform.forward.z), new Vector2(vecToMe.x, vecToMe.z));

        return isActiveAndEnabled && angle <= m_coneAngle && m_condition.Execute() ;
    }

    public void TryInteract(PlayerController a_player)
    {
        DebugPrint("TryInteract");
        if (!m_isToggleUse || !IsUsing)
        {
            Interact(a_player);
        }

    }


    protected virtual void Interact(PlayerController a_player)
    {
        DebugPrint("Interact");

        if (m_isToggleUse)
        {
          //  a_player.Freeze = true;
            IsUsing = true;
            m_user = a_player;
        }
    }

    public void TryRelease()
    {
        if (IsUsing)
        {
            Release();
        }
    }

    protected virtual void Release()
    {
        IsUsing = false;
      /*  m_user.Freeze = false;
        m_user.ResetInteractable();*/
    }

    protected virtual void Start()
    {
        m_identity = GetComponent<ObjectIdentityHandler>();

        GameObject interactableUI = Instantiate(m_InteractableUIPrefab.gameObject, transform.position + m_UIOffset, Quaternion.identity);
        interactableUI.transform.SetParent(transform);
        m_InteractableUI = interactableUI.GetComponent<InteractableUI>();
        m_InteractableUI.Display(false);

#if UNITY_EDITOR
        if (m_debugName == "")
        {
            m_debugName = m_identity.GetName();
        }
        m_InteractableUI.name = "Interractable UI " + m_debugName;
#endif
    }

    private void Update()
    {
        if(IsUsing || !m_isToggleUse)
        {
            ApplyUpdate();
        }
    }

    protected virtual void ApplyUpdate()
    {

    }

    protected void DebugPrint(string a_message)
    {
        Debug.Log("["+ m_debugName + "] : " + a_message);
    }

}
