using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableUI : MonoBehaviour
{
    [SerializeField]
    string m_text = "Press E";

    [SerializeField]
    TextMeshProUGUI m_textField;

    public void Start()
    {
        m_textField.text = m_text;
    }

    public void SetText(string a_text)
    {
        m_text = a_text;
        m_textField.text = a_text;
    }


    public void Display(bool a_bool)
    {
        m_textField.enabled = a_bool;
        Actualize();
    }

    private void Actualize()
    {
        SetText(m_text);
    }
}
