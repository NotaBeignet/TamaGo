using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class DialogueWindowMode : DialogueContainer
{
    int m_windowIndex;
    public DialogueWindowMode(XmlNode a_node) : base(a_node)
    {
        m_windowIndex = int.Parse(a_node.Attributes["id"].Value);
    }


    public override bool Read()
    {
        DialogueManager.Instance.SetWindow(m_windowIndex);
        return base.Read();
    }

}
