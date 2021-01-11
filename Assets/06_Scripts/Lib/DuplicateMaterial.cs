using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateMaterial : MonoBehaviour
{

    [SerializeField]
    List<int> m_materialIndexes = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        foreach(int index in m_materialIndexes)
        {
            Material mat = new Material(renderer.materials[index].shader);
            renderer.materials[index] = mat;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
