using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entry<T> : ExtendMonobehaviour
{    public abstract void Fill(T a_object);
}
