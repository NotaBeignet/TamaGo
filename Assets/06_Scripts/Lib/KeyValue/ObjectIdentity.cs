using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ObjectIdentity", menuName = "Object Identity", order = 52)]
public class ObjectIdentity : ScriptableObject
{
    public void Lock(bool a_value)
    {
        KeyValueManager.Instance.KeyValueData.SetValueBool(name + "Lock", a_value);
    }

    public bool IsLock()
    {
        return KeyValueManager.Instance.KeyValueData.GetValueBool(name + "Lock");
    }

    public bool HasLockInfos()
    {
        return KeyValueManager.Instance.KeyValueData.HasValueBool(name + "Lock");
    }

    public void Open(bool a_value)
    {
        KeyValueManager.Instance.KeyValueData.SetValueBool(name + "IsOpen", a_value);
    }

    public bool IsOpen()
    {
        return KeyValueManager.Instance.KeyValueData.GetValueBool(name + "IsOpen");
    }

    public bool HasOpenInfos()
    {
        return KeyValueManager.Instance.KeyValueData.HasValueBool(name + "IsOpen");
    }

    public void Taken(bool a_value)
    {
        KeyValueManager.Instance.KeyValueData.SetValueBool(name + "IsTaken", a_value);
    }

    public bool IsTaken()
    {
        return KeyValueManager.Instance.KeyValueData.GetValueBool(name + "IsTaken");
    }

    public bool HasTakenInfos()
    {
        return KeyValueManager.Instance.KeyValueData.HasValueBool(name + "IsTaken");
    }

    public void SpokeTo(bool a_value)
    {
        KeyValueManager.Instance.KeyValueData.SetValueBool(name + "IsSpokenTo", a_value);
    }

    public bool IsSpokenTo()
    {
        return KeyValueManager.Instance.KeyValueData.HasValueBool(name + "IsSpokenTo");
    }
}
