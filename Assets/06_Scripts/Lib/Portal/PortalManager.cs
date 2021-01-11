using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PortalManager : Singleton<PortalManager>
{

    List<Portal> m_portals = new List<Portal>();

    public bool AddPortal(Portal a_portal)
    {
         if(m_portals.Find((o) => (o.Scene1 == a_portal.Scene1 && o.Scene2 == a_portal.Scene2) || (o.Scene1 == a_portal.Scene2 && o.Scene2 == a_portal.Scene1)) != null)
        {
            m_portals.Add(a_portal);
            return false;
        }
        else
        {
            m_portals.Add(a_portal);
            return true;
        }

    }


    public void RemovePortal(Portal a_portal)
    {
        m_portals.Remove(a_portal);
        Portal portal = (m_portals.Find((o) => (o.Scene1 == a_portal.Scene1 && o.Scene2 == a_portal.Scene2) || (o.Scene1 == a_portal.Scene2 && o.Scene2 == a_portal.Scene1)));
        if(portal != null)
        {
            portal.IsUsable = true;
        }

    }

}
