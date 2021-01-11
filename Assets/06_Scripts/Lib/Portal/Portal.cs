using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{

    [SerializeField]
    TriggerObservable m_scene1Trigger;
    [SerializeField]
    TriggerObservable m_scene2Trigger;
    [SerializeField]
    TriggerObservable m_middleTrigger;

    string m_scene1;
    [SerializeField]
    string m_scene2;

    [SerializeField]
    List<GameObject> m_linkedObjects;


    [SerializeField]
    Condition m_condition;


 


    bool m_isUsable = false;

    bool m_insideMiddle = false;
    int m_lastTriggerInside = -1;

    bool m_scene2Loaded = false;
    bool m_scene2Loading = false;

    public string Scene1 { get => m_scene1; set => m_scene1 = value; }
    public string Scene2 { get => m_scene2; set => m_scene2 = value; }
    public bool IsUsable { get => m_isUsable; set { m_isUsable = value; Actualize(); } }

    private void Start()
    {

        m_scene1 = gameObject.scene.name;

        IsUsable = PortalManager.Instance.AddPortal(this);


        m_scene1Trigger.Register(null, OnStaySide, null);
        m_scene2Trigger.Register(null, OnStaySide, null);
        m_middleTrigger.Register(OnEnterMiddle, OnStayMiddle, OnExitMiddle);
    }




    private void OnStaySide(TriggerObservable a_trigger, Collider a_other)
    {

        if (IsUsable && a_other.CompareTag("Player"))
        {
            if (a_trigger == m_scene1Trigger)
            {
                m_lastTriggerInside = 0;
            }
            else
            {
                m_lastTriggerInside = 1;
            }

            if (m_scene2Loaded && !m_insideMiddle)
            {
                Unload(() => { return a_trigger == m_scene1Trigger; });
            }
        }

    }


    private void OnEnterMiddle(TriggerObservable a_trigger, Collider a_other)
    {
        if (IsUsable && a_other.CompareTag("Player"))
        {
            m_insideMiddle = true;
            if (!m_scene2Loaded && !m_scene2Loading && m_condition.Execute())
            {
                SceneManager.LoadSceneAsync(m_scene2, LoadSceneMode.Additive);
                Reposition();
                m_scene2Loading = true;
            }
        }
    }

    private void OnStayMiddle(TriggerObservable a_trigger, Collider a_other)
    {
        if (!m_scene2Loading && !m_scene2Loaded &&  IsUsable && a_other.CompareTag("Player") && m_condition.Execute())
        {
            SceneManager.LoadSceneAsync(m_scene2, LoadSceneMode.Additive);
            Reposition();
            m_scene2Loading = true;
        }
    }


    private void OnExitMiddle(TriggerObservable a_trigger, Collider a_other)
    {
        if (IsUsable  && a_other.CompareTag("Player"))
        {
            m_insideMiddle = false;

            Utils.TriggerNextFrame(() =>
           {
               if (m_scene2Loaded && !m_scene1Trigger.IsInside("Player") && !m_scene2Trigger.IsInside("Player") && m_lastTriggerInside != -1)
               {
                 Unload(() => { return m_lastTriggerInside == 0; });                
               }
           });
        }
    }

    private void Unload(System.Func<bool> a_comparison)
    {
        Scene scene;
        if (a_comparison())
        {
            scene = SceneManager.GetSceneByName(Scene2);
        }
        else
        {
            scene = SceneManager.GetSceneByName(Scene1);
            IsUsable = false;
        }
        SceneManager.UnloadSceneAsync(scene);
        m_scene2Loaded = false;
    }

    private void Reposition()
    {
        Scene scene = SceneManager.GetSceneByName(m_scene2);
        if (scene.isLoaded)
        {
            Vector3 offset = Vector3.zero;
            Quaternion offsetRotation = Quaternion.identity;
            GameObject[] roots = scene.GetRootGameObjects();
            Portal[] portals;
            GameObject root = null;
            Portal portal = null;

            for(int i =0; i < roots.Length; ++i)
            {
                if (roots[i].name == "Root")
                {
                    root = roots[i];
                    break;
                }
            }

            portals = root.GetComponentsInChildren<Portal>();

            for (int i =0; i < portals.Length; ++i)
            {
                portal = portals[i].GetComponent<Portal>();
                if((Scene1 == portal.Scene1 && Scene2 == portal.Scene2) || (Scene1 == portal.Scene2 && Scene2 == portal.Scene1))
                {
                    offsetRotation = Quaternion.FromToRotation(portal.transform.forward * -1 ,transform.forward);

                    break;
                }             
            }

            root.transform.rotation = (root.transform.rotation * offsetRotation);

            offset = transform.position - portal.transform.position;

            root.transform.position += offset;

            m_scene2Loaded = true;
            m_scene2Loading = false;
        }
        else
        {
            Utils.TriggerNextFrame(Reposition);
        }

    }

    private void Actualize()
    {
        foreach(GameObject go in m_linkedObjects)
        {
            go.SetActive(IsUsable);
        }
    }



    private void OnDestroy()
    {
        PortalManager.Instance.RemovePortal(this);
    }

}

