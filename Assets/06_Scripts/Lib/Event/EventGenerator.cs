#if UNITY_EDITOR

using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

//[InitializeOnLoad]
public static class EventGenerator
{

    private static List<List<string>> m_events = new List<List<string>>();

    private static string m_path = "/06_Scripts/Lib/Event/EventManager.cs";

    static EventGenerator()
    {
        m_events = RetrieveEvents();

        WriteCodeFile();
    }

 
    private static List<List<string>> RetrieveEvents()
    {
        string path = string.Concat(Application.dataPath, Path.DirectorySeparatorChar, m_path);

        List<List<string>> res = new List<List<string>>();


        try
        {
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {

                using (StreamReader reader = new StreamReader(stream))
                {
                    string line = reader.ReadLine();
                    bool inList = false;

                    Regex regex = new Regex("(\\w+)");
                    Match match;
                    int index = -1;

                    while (line != null)
                    {
                        //If we are at the end, go out
                        if (line.Contains("##END##"))
                        {
                            break;
                        }

                        //inside the list, read the line
                        if (inList) 
                        {

                            match = regex.Match(line);
                            bool first = true;
                            while (match.Success)
                            { 
                                if (first)
                                {
                                    res.Add(new List<string>());
                                    index++;
                                    first = false;
                                }
                                res[index].Add(match.Value);
                                match = match.NextMatch();
                            }
                            
                        }

                        //We are at the beginning, let's read
                        if (line.Contains("##BEGIN##"))
                        {
                            inList = true;
                        }
                        


                        line = reader.ReadLine();
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }

        return res;
    }


 
    private static void WriteCodeFile()
    {

        // the path we want to write to
        string path = string.Concat(Application.dataPath, Path.DirectorySeparatorChar, m_path);

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("// ----- AUTO GENERATED CODE ----- //");

        for (int i = 0; i < m_events.Count; i++)
        {

            //Action<> line
            StringBuilder actionBuilder = new StringBuilder();
            //Function parameter line
            StringBuilder parametersBuilder = new StringBuilder();
            //Call to function parameter line
            StringBuilder inputBuilder = new StringBuilder();

            builder.AppendLine("");
            builder.AppendLine("");
            builder.AppendLine("");
            builder.AppendLine("");
            builder.AppendLine("// --- EVENT --- " + m_events[i][0] + " --- //");
            builder.AppendLine("");
            builder.AppendLine("");

            //retrieve the parameters, knowing 0 is the name of the event
            actionBuilder.Append("Action<object");
            for (int j = 1; j < m_events[i].Count; j++)
            {
                actionBuilder.Append(", " + m_events[i][j]);
                parametersBuilder.Append(", " + m_events[i][j] + " a_" + j + char.ToLower(m_events[i][j][0]) + m_events[i][j].Substring(1));
                inputBuilder.Append(", a_"+ j + char.ToLower(m_events[i][j][0]) + m_events[i][j].Substring(1));
            }
            actionBuilder.Append(">");



            //Event variables
            builder.Append("\tprotected event ");
            builder.Append(actionBuilder.ToString());
            builder.AppendLine(" On" + m_events[i][0] + ";");



            //functions
            builder.AppendLine("\tpublic void RegisterOn" + m_events[i][0] + "(" + actionBuilder.ToString() + " a_action)");
            builder.AppendLine("\t{");
            builder.AppendLine("\t\tOn" + m_events[i][0] + " += a_action;");
            builder.AppendLine("\t}");

            builder.AppendLine("");
            builder.AppendLine("");


            builder.AppendLine("\tpublic void UnRegisterOn" + m_events[i][0] + "(" + actionBuilder.ToString() + " a_action)");
            builder.AppendLine("\t{");
            builder.AppendLine("\t\tOn" + m_events[i][0] + " -= a_action;");
            builder.AppendLine("\t}");

            builder.AppendLine("");
            builder.AppendLine("");      

            builder.AppendLine("\tpublic void InvokeOn" + m_events[i][0] + "(object a_sender" + parametersBuilder.ToString() + ")");
            builder.AppendLine("\t{");
            builder.AppendLine("\t\tif(On" + m_events[i][0] + " != null)");
            builder.AppendLine("\t\t{");
            builder.AppendLine("\t\t\tOn" + m_events[i][0] + ".Invoke(a_sender" + inputBuilder.ToString() + ");");
            builder.AppendLine("\t\t}");
            builder.AppendLine("\t}");
        }
        builder.AppendLine("// ----- END AUTO GENERATED CODE ----- //");


        UtilsCodeGenerator.ReplaceClassInFile(path, "EventManager", builder.ToString());
    }

    /* private static void Wait()
   {
       // if nothing has changed return
       if (!m_hasChanged)
           return;

       // if the time delta between now and the last change, is greater than the time we schould wait Than write the file
       if (EditorApplication.timeSinceStartup - m_startTime > m_timeToWait)
       {
           // WriteCodeFile();
           Debug.Log("CHANGED");
           m_hasChanged = false;
       }
   }**/


    /*private static void Update()
    {
        // returns if we are in play mode
        if (Application.isPlaying == true)
            return;

        Wait();

        // temp array that hold new tags
        List<List<string>> newEvents = RetrieveEvents();
        // check if the lenght is not the same
        if (m_events.Count != newEvents.Count)
        {
            m_hasChanged = true;
        }
        else
        {
            // loop thru all new tags and compare them to the old ones
            for (int i = 0; i < m_events.Count; i++)
            {

                if(m_events[i].Count != newEvents[i].Count)
                {
                    m_hasChanged = true;
                    break;
                }

                for (int j = 0; j < m_events[i].Count; j++)
                {
                    if (!string.Equals(m_events[i][j], newEvents[i][j]))
                    {
                        m_hasChanged = true;
                        break;
                    }
                }
            }
        }

        if (m_hasChanged)
        {
            m_events = newEvents;
            m_startTime = EditorApplication.timeSinceStartup;
        }

    }*/


}

#endif