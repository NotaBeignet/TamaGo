using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;


[Serializable]
public class Condition
{
    [SerializeField]
    string m_name;

    [SerializeField]
    bool m_isList;

    [SerializeField]
    bool m_isOr;

    [SerializeField]
    bool m_isNot;

    [SerializeField]
    List<Condition> m_conditions;

    [SerializeField]
    ConditionCommand m_conditionCommand;


    public void SetParam(string a_name, object a_value)
    {
        foreach (Condition condition in m_conditions)
        {
            condition.SetParam(a_name, a_value);
        }

        if(m_conditionCommand != null)
        {
            m_conditionCommand.SetParam(a_name, a_value);
        }
    }


    public bool Execute()
    {
        bool res = true;
        if (m_isList)
        {
            if (m_isOr)
            {
                res = OrExecute();
            }
            else
            {
                res = AndExecute();
            }
        }
        else
        {
            if (m_conditionCommand != null)
            {
                res = m_conditionCommand.Execute();
            }
        }

        return m_isNot ? !res : res ;
    }

    private bool AndExecute()
    {
        bool res = true;
        foreach (Condition condition in m_conditions)
        {
            res = condition.Execute();
            if (!res)
            {
                break;
            }
        }

        return res;
    }

    private bool OrExecute()
    {
        bool res = false;
        foreach (Condition condition in m_conditions)
        {
            res = condition.Execute();
            if (res)
            {
                break;
            }
        }

        return res;
    }


    public void Reset()
    {
        m_isList = false;
        m_isOr = false;
        m_isNot = false;
        m_conditionCommand = null;
    }

}


