using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class DialogueContainer : DialogueEntry
{
    List<DialogueEntry> m_entries = new List<DialogueEntry>();
    int m_dialogueStep = 0;


    public DialogueContainer(XmlNode a_node)
    {
        foreach (XmlNode node in a_node.ChildNodes)
        {
            m_entries.Add(DialogueParser.GetEntriesInChild(this, node));
        }
    }

    public override bool Read()
    {
        bool res = false;
        if (m_entries[m_dialogueStep].Read())
        {
            m_dialogueStep++;
            if(m_dialogueStep >= m_entries.Count)
            {
                res = true;
            }
        }
        return res;
    }
}
