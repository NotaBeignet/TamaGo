using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class KeyValueManager : Singleton<KeyValueManager>
{
    public KeyValueData KeyValueData { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        KeyValueData = new KeyValueData();
    }

}

public class KeyValueData
{

    Dictionary<string, bool> m_bools = new Dictionary<string, bool>();
    Dictionary<string, int> m_ints = new Dictionary<string, int>();
    Dictionary<string, float> m_floats = new Dictionary<string, float>();
    Dictionary<string, string> m_strings = new Dictionary<string, string>();



    public bool GetValueBool(string a_key)
    {
        bool res = false;

        m_bools.TryGetValue(a_key, out res);
            
        return res;
    }

    public int GetValueInt(string a_key)
    {
        int res = 0;

        m_ints.TryGetValue(a_key, out res);

        return res;
    }

    public float GetValueFloat(string a_key)
    {
        float res = 0;

        m_floats.TryGetValue(a_key, out res);

        return res;
    }

    public string GetValueString(string a_key)
    {
        string res = "";

        m_strings.TryGetValue(a_key, out res);

        return res;
    }


    public bool HasValueBool(string a_key)
    {
        return m_bools.ContainsKey(a_key);
    }

    public bool HasValueInt(string a_key)
    {
        return m_ints.ContainsKey(a_key);
    }

    public bool HasValueFloat(string a_key)
    {
        return m_floats.ContainsKey(a_key);
    }

    public bool HasValueString(string a_key)
    {
        return m_strings.ContainsKey(a_key);
    }



    public void SetValueBool(string a_key, bool a_value)
    {
        m_bools[a_key] = a_value;
    }

    public void SetValueInt(string a_key, int a_value)
    {
        m_ints[a_key] = a_value;
    }

    public void SetValueFloat(string a_key, float a_value)
    {
        m_floats[a_key] = a_value;
    }

    public void SetValueString(string a_key, string a_value)
    {
        m_strings[a_key] = a_value;
    }



}
