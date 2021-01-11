using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraStrategy
{
    protected GameObject m_target;
    protected Camera m_camera;

    private float verticalSpeed;
    private float horizontalSpeed;

    public float VerticalSpeed { get => verticalSpeed; set => verticalSpeed = value; }
    public float HorizontalSpeed { get => horizontalSpeed; set => horizontalSpeed = value; }




    protected CameraStrategy()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void SetTarget(GameObject a_target)
    {
        m_target = a_target;
    }

    public virtual void Initialize(Camera a_camera)
    {
        m_camera = a_camera;
    }

    public virtual void Release()
    { 

    }


    public virtual void SynchronizePos()
    {

    }



}
