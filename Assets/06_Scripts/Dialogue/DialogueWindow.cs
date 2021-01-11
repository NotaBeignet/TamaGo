using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DialogueWindow : MonoBehaviour
{
    TextMeshProUGUI m_characterZone;
    TextMeshProUGUI m_textZone;

    private void Awake()
    {
        m_characterZone = transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        m_textZone = transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void ResetWindow()
    {
        m_characterZone.text = "";
        m_textZone.text = "";
    }

    internal void DisplayLine(string a_character, string a_line)
    {
        m_characterZone.text = a_character;
        m_textZone.text = a_line;
    }
}
