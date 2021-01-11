using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class DialogueLine : DialogueEntry
{
    string m_character;
    string m_line;

    public DialogueLine(XmlNode a_node)
    {
        Debug.Log("[Dialogue] Create " + a_node.InnerText);
        m_character = a_node.Attributes["character"].Value;
        m_line = a_node.InnerText;
    }

    public override bool Read()
    {
        DialogueManager.Instance.DisplayLine(m_character, m_line);
        return true;
    }
}
