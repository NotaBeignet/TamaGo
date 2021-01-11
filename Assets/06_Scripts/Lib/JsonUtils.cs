using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonUtils : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		
	}

    void WriteData(string fileName, string content)
    {
        TextAsset textAsset = (TextAsset)Resources.Load(fileName); // Don't include the .json extension
        string jsonString = textAsset.text;
        JsonUtility.FromJsonOverwrite(jsonString, this);
    }
    
    void LoadData(string fileName)
    {
        TextAsset textAsset = (TextAsset)Resources.Load(fileName); // Don't include the .json extension
        string jsonString = textAsset.text;
        JsonUtility.FromJsonOverwrite(jsonString, this);
    }

}
