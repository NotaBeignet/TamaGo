using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionKeyValueCommand : ConditionCommand
{
    [HideInInspector]
    [SerializeField]
    string a_keyValue;


    public bool BoolKeyValueIsTrue(string a_keyValue)
    {
        return KeyValueManager.Instance.KeyValueData.GetValueBool(a_keyValue);
    }

}
