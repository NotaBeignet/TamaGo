using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFPS : CameraStrategy
{
    Vector3 m_anchorPoint;
    bool m_centerOnTarget = true;
    bool m_rotateTarget = false;



    public CameraFPS(float a_verticalSpeed = 3.5f, float a_horizontalSpeed = 5.0f, bool a_rotateTarget = false)
    {
        InitValues(a_verticalSpeed, a_horizontalSpeed, a_rotateTarget);
    }

    public CameraFPS(Vector3 a_anchorPoint, float a_verticalSpeed = 3.5f, float a_horizontalSpeed = 5.0f, bool a_rotateTarget = false)
    {
        InitValues(a_verticalSpeed, a_horizontalSpeed, a_rotateTarget);
        m_anchorPoint = a_anchorPoint;
        m_centerOnTarget = false;
    }



    private void InitValues(float a_verticalSpeed, float a_horizontalSpeed, bool a_rotateTarget)
    {
        VerticalSpeed = a_verticalSpeed;
        m_rotateTarget = a_rotateTarget;
        HorizontalSpeed = a_horizontalSpeed;
    }


    public override void Update()
    {
        if (m_target && Cursor.lockState == CursorLockMode.Locked)
        {
            float h = HorizontalSpeed * Input.GetAxis("Mouse X");
            h += (HorizontalSpeed -0.75f) * Input.GetAxis("Horizontal");

            float v = VerticalSpeed * Input.GetAxis("Mouse Y")* -1.0f;
            v += (VerticalSpeed -0.5f) * Input.GetAxis("Vertical")* -1.0f;

            m_camera.transform.Rotate(v, h, 0);
            m_camera.transform.localRotation = Quaternion.Euler(m_camera.transform.localRotation.eulerAngles.x, m_camera.transform.localRotation.eulerAngles.y, 0);
            UpdatePosition();
            if (m_rotateTarget)
            {
                m_target.transform.localRotation = m_camera.transform.localRotation;
            }
        }

    }

    public override void SetTarget(GameObject a_target)
    {
        base.SetTarget(a_target);
    }

    public override void Initialize(Camera a_camera)
    {
        base.Initialize(a_camera);
        
        UpdatePosition();
        m_camera.transform.localRotation = new Quaternion();// m_target.transform.forward;

        Camera.main.cullingMask = (Camera.main.cullingMask ^ LayerMask.GetMask("Player"));
    }

    void UpdatePosition()
    {
        if (m_centerOnTarget)
        {
            m_anchorPoint = m_target.transform.position;
        }
        m_camera.transform.position = m_anchorPoint;
    }

    public override void Release()
    {
        base.Release();
        Camera.main.cullingMask = (Camera.main.cullingMask ^ LayerMask.GetMask("Player"));
    }

}

