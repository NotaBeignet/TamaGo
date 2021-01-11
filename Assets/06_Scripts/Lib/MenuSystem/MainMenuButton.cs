using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => MenuManager.Instance.BackToMainMenu());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
