using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPosition : MonoBehaviour
{
    protected virtual void Start()
    {
        transform.parent = null;
        transform.rotation = Quaternion.identity;
    }
}
