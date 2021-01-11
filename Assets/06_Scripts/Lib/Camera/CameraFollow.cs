using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : CameraStrategy
{
    Vector3 m_orientation;

    float m_speed;

    Vector3 m_offset;



    public CameraFollow(Vector3 a_offset, Vector3 a_orientation, float a_speed)
    {
        m_orientation = a_orientation;
        m_offset = a_offset;
        m_speed = a_speed;
    }



    public override void Update()
    {
        if (m_target)
        {
            m_camera.transform.position = m_target.transform.position + m_offset;
            m_camera.transform.localRotation = Quaternion.Euler(m_orientation);

            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                m_offset += m_camera.transform.forward * m_speed;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                m_offset -= m_camera.transform.forward * m_speed;
            }

        }

    }

}

