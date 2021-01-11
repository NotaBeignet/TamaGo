using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionObjectIdentityCommand : ConditionCommand
{
    [HideInInspector]
    [SerializeField]
    ObjectIdentity a_objectIdentity;


    public bool IsOpen(ObjectIdentity a_objectIdentity)
    {
        return a_objectIdentity.IsOpen();
    }

    public bool IsLocked(ObjectIdentity a_objectIdentity)
    {
        return a_objectIdentity.IsLock();
    }
    
    public bool IsSpokenTo(ObjectIdentity a_objectIdentity)
    {
        return a_objectIdentity.IsSpokenTo();
    }

}
