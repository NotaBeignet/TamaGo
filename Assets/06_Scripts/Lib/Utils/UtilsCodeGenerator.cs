using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class UtilsCodeGenerator
{
    static public void ReplaceClassInFile(string filePath, string searchClass, string replaceText)
    {

        //TODO - need to check if it's useful to put "using" arround this
        try
        {
            StreamWriter writer = new StreamWriter(filePath + ".temp");
            StreamWriter writerSave = new StreamWriter(filePath + ".tempSave");
            StreamReader reader = new StreamReader(filePath);

            string line = reader.ReadLine();
            while (line != null && !line.Contains("public class " + searchClass))
            {
                writer.WriteLine(line);
                writerSave.WriteLine(line);
                line = reader.ReadLine();
            }

            while (line != null && !line.Contains("{"))
            {
                writer.WriteLine(line);
                writerSave.WriteLine(line);
                line = reader.ReadLine();
            }

            writer.WriteLine(line);
            writerSave.WriteLine(line);

            int countbracket = 0;
            if (line != null)
            {
                do
                {
                    if (line.Contains("{"))
                    {
                        countbracket++;
                    }
                    else if (line.Contains("}"))
                    {
                        countbracket--;
                    }

                    if (countbracket != 0)
                    {
                        line = reader.ReadLine();
                    }

                } while (countbracket != 0 && line != null);
            }

            line = "\n" + replaceText + "\n" + line;

            while (line != null)
            {
                writer.WriteLine(line);
                writerSave.WriteLine(line);
                line = reader.ReadLine();
            }



            reader.Close();
            writer.Close();
            writerSave.Close();

            writer = new StreamWriter(filePath);
            reader = new StreamReader(filePath + ".temp");
            line = reader.ReadLine();

            while (line != null)
            {
                writer.WriteLine(line);
                line = reader.ReadLine();
            }

            reader.Close();
            writer.Close();

            File.Delete(filePath + ".temp");
            File.Delete(filePath + ".tempSave");
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }

    }
}



/* EXAMPLE FROM : https://forum.unity.com/threads/code-generation-with-unity.233661/
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
 
[InitializeOnLoad]
public static class TagCodeGenerator
{
    // an array that hold all tags
    private static string[] _tags;
    // a flag if the dataset has changed
    private static bool _hasChanged = false;
    // time when we start to count
    private static double _startTime = 0.0;
    // the time that should elapse between the change of tags and the File write
    // this is importend because changed are triggered as soon as you start typing and this can cause lag
    private static double _timeToWait = 1.0;

    static TagCodeGenerator()
    {
        //subscripe to event
        EditorApplication.update += Update;
        // get tags
        _tags = InternalEditorUtility.tags;
        // write file
        WriteCodeFile();

    }

    private static void Update()
    {
        // returns if we are in play mode
        if (Application.isPlaying == true)
            return;

        Wait();

        // temp array that hold new tags
        string[] newTags = InternalEditorUtility.tags;
        // check if the lenght is not the same
        if (newTags.Length != _tags.Length)
        {
            _tags = newTags;
            _hasChanged = true;
            _startTime = EditorApplication.timeSinceStartup;
            return;
        }
        else
        {
            // loop thru all new tags and compare them to the old ones
            for (int i = 0; i < newTags.Length; i++)
            {
                if (string.Equals(newTags[i], _tags[i]) == false)
                {
                    _tags = newTags;
                    _hasChanged = true;
                    _startTime = EditorApplication.timeSinceStartup;
                    return;
                }
            }
        }

    }

    private static void Wait()
    {
        // if nothing has changed return
        if (_hasChanged == false)
            return;

        // if the time delta between now and the last change, is greater than the time we schould wait Than write the file
        if (EditorApplication.timeSinceStartup - _startTime > _timeToWait)
        {
            WriteCodeFile();
            _hasChanged = false;
        }
    }


    // writes a file to the project folder
    private static void WriteCodeFile()
    {

        // the path we want to write to
        string path = string.Concat(Application.dataPath, Path.DirectorySeparatorChar, "Tags.cs");

        try
        {
            // opens the file if it allready exists, creates it otherwise
            using (FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("// ----- AUTO GENERATED CODE ----- //");
                    builder.AppendLine("public static class Tags");
                    builder.AppendLine("{");
                    foreach (string tag in _tags)
                    {
                        builder.AppendLine(string.Format("\tpublic static readonly string {0} = \"{0}\";", tag));
                    }

                    builder.AppendLine("}");
                    writer.Write(builder.ToString());
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);

            // if we have an error, it is certainly that the file is screwed up. Delete to be save
            if (File.Exists(path) == true)
                File.Delete(path);
        }

        AssetDatabase.Refresh();
    }
}
*/
