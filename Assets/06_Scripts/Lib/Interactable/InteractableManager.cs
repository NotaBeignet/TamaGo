using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class InteractableManager : MonoBehaviour
{


    List<Interactable> m_interactables;

    int m_activeInteractable = -1;
    int m_activeTriggers = 0;

    void Start()
    {
        m_interactables = GetComponents<Interactable>().ToList<Interactable>();
        m_interactables = m_interactables.OrderByDescending(o => o.Priotity).ToList();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ++m_activeTriggers;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        //ensure is the player and we are not curently using an interactable
        if (other.gameObject.CompareTag("Player") && (m_activeInteractable < 0 || !m_interactables[m_activeInteractable].IsUsing)) 
        {
            int i = 0;
            PlayerController player = other.GetComponent<PlayerController>();
            foreach (Interactable interactable in m_interactables)
            {
                if (interactable.IsInteractable(player))
                {
                    if(m_activeInteractable == i)
                    {
                        interactable.OnStay(player);
                    }
                    else
                    {
                        if(m_activeInteractable >= 0)
                        {
                            m_interactables[m_activeInteractable].OnExit(player);
                        }

                     /*   if (player.SetInteractable(m_interactables[i]))
                        {*/
                            interactable.OnEnter(player);
                            m_activeInteractable = i;
                       // }
                    }

                    break;
                }
                ++i;
            }
        }
    }

    public void Release(PlayerController a_player)
    {
        m_interactables[m_activeInteractable].OnExit(a_player);
        m_activeInteractable = -1;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && --m_activeTriggers == 0 && m_activeInteractable >= 0)
        {
            //other.GetComponent<PlayerController>().ResetInteractable();
        }
    }



}
