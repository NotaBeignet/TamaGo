using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

public class DialogueParser
{

    public DialogueContainer ParseXMLDialogue(string a_path)
    {
        a_path = "Assets/Resources/Dialogues/" + a_path;
        XmlDocument xmlDialogue = new XmlDocument();
        xmlDialogue.Load(a_path);
        DialogueContainer dialogue = new DialogueContainer(xmlDialogue.ChildNodes[1]);
                
        return dialogue;
    }

    public static DialogueEntry GetEntriesInChild(DialogueContainer a_dialogue, XmlNode a_node)
    {
        string type = a_node.Name;
        DialogueEntry res = null;
        switch (type)
        {
            case "window":
                res = new DialogueWindowMode(a_node);
                break;
            case "line":
                res = new DialogueLine(a_node);
                break;
            default:
                Debug.LogError("[Dialogue] Not a valid format " + type);
                break;
        }

        return res;
    }
}
