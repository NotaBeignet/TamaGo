using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLoopStop : MonoBehaviour
{

    Animator m_animator;

    Animator Animator
    {
        get
        {
            if(m_animator == null)
            {
                m_animator = GetComponent<Animator>();
            }
            return m_animator;
        }
    }

    [SerializeField]
    bool m_isUnscaled;
    public bool IsUnscaled { get => m_isUnscaled; set => m_isUnscaled = value; }

    // Start is called before the first frame update
    void Start()
    {
        Animator.enabled = false;
    }


    public void StartAnim()
    {
        Animator.enabled = true;
        Animator.SetBool("ForceStop", false);
        Animator.SetBool("Enabled", true);
    }

    public void StopAnim()
    {
        Animator.SetBool("Enabled", false);
    }

    public void ForceStop()
    {
        Animator.SetBool("ForceStop", true);
        StopAnim();
    }



    // Update is called once per frame
    void Update()
    {
        if (IsUnscaled && Time.timeScale != 0)
        {
            m_animator.speed = 1.0f / Time.timeScale;
          //  m_animator.SetFloat("SpeedMultiplier", 1.0f / Time.timeScale);
        }
    }
}
