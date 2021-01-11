using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ListContainer<T,U> : ExtendMonobehaviour
{
    [SerializeField]
    Entry<U> m_entryPrefab;

    [SerializeField]
    Transform m_containerEntry;

    T m_object;

    public void Fill(T a_object)
    {
        /*if (a_object == null)
        {
            GetComponent<ExtendCanvas>().enabled = false;
            return;
        }

        GetComponent<ExtendCanvas>().enabled = true;*/

        Utils.DestroyChilds(m_containerEntry);

        foreach (U obj in RetrieveObjects(a_object))
        {
            Entry<U> entry = Instantiate(m_entryPrefab, m_containerEntry);
            entry.Fill(obj);
        }

        m_object = a_object;
    }

    public void Actualize()
    {
        Fill(m_object);
    }

    protected abstract List<U> RetrieveObjects(T a_object);
}
