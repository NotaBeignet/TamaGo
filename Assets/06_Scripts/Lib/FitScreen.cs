using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fitscreen : MonoBehaviour
{

    bool m_isRotate;
    void Start()
    {

        m_isRotate = transform.rotation.z != 0;

        ResizeSpriteToScreen();

    }


    void ResizeSpriteToScreen()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        transform.localScale = new Vector3(1, 1, 1);

        var width = sr.sprite.bounds.size.x;
        var height = sr.sprite.bounds.size.y;

        if (m_isRotate)
        {
            float temp = width;
            width = height;
            height = temp;
        }

        var worldScreenHeight = Camera.main.orthographicSize * 2.0;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        float scale = Mathf.Max((float)worldScreenWidth / width, (float)worldScreenHeight / height);
        transform.localScale = new Vector2(scale, scale);
    }


}