using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Camera25DParam
{
    [SerializeField]
    float m_FOV;

    [SerializeField]
    Vector3 m_offset;

    [SerializeField]
    float m_speed;


    [SerializeField]
    float m_angleTargetPercent;

    [SerializeField]
    float m_maxYawAngle;

    [SerializeField]
    float m_yawSpeed;

    [SerializeField]
    float m_maxRollAngle;

    [SerializeField]
    float m_rollSpeed;


    public float Speed { get => m_speed; set => m_speed = value; }
    public Vector3 Offset { get => m_offset; set => m_offset = value; }
    public float FOV { get => m_FOV; set => m_FOV = value; }
    public float MaxYawAngle { get => m_maxYawAngle; set => m_maxYawAngle = value; }
    public float YawSpeed { get => m_yawSpeed; set => m_yawSpeed = value; }
    public float MaxRollAngle { get => m_maxRollAngle; set => m_maxRollAngle = value; }
    public float RollSpeed { get => m_rollSpeed; set => m_rollSpeed = value; }
    public float AngleTargetPercent { get => m_angleTargetPercent; set => m_angleTargetPercent = value; }
}

public class Camera25D : CameraStrategy
{
    Camera25DParam m_parameters;
    
    public Camera25D(Camera25DParam a_param)
    {
        m_parameters = a_param;
    }

    public override void Initialize(Camera a_camera)
    {
        base.Initialize(a_camera);

        m_camera.fieldOfView = m_parameters.FOV;
    }

    public override void Update()
    {
        base.Update();
        if (m_target)
        {
            UpdatePos(m_parameters.Speed, m_parameters.YawSpeed, m_parameters.RollSpeed);
        }
    }

    void UpdatePos(float a_speed, float a_yawSpeed, float a_rollSpeed)
    {
        m_camera.transform.parent.position = Vector3.Lerp(m_camera.transform.parent.position,
            new Vector3(m_target.transform.position.x + m_parameters.Offset.x, m_target.transform.position.y + m_parameters.Offset.y, m_target.transform.position.z + m_parameters.Offset.z),
            Time.deltaTime * a_speed);

        float currentzAngle = ((m_camera.transform.parent.eulerAngles.z % 180) - (180 * ((int)m_camera.transform.parent.eulerAngles.z / 180)));
        float currentYAngle = ((m_camera.transform.parent.eulerAngles.y % 180) - (180 * ((int)m_camera.transform.parent.eulerAngles.y / 180)));


        float targetAngle = ((m_target.transform.eulerAngles.z  % 180) - (180 * ((int)m_target.transform.eulerAngles.z / 180))) * m_parameters.AngleTargetPercent;


        float zAngle = Mathf.Clamp(Mathf.Lerp(currentzAngle, -targetAngle, Time.deltaTime * a_yawSpeed), -m_parameters.MaxYawAngle, m_parameters.MaxYawAngle);
        float yAngle = Mathf.Clamp(Mathf.Lerp(currentYAngle, targetAngle, Time.deltaTime * a_rollSpeed), -m_parameters.MaxRollAngle, m_parameters.MaxRollAngle);

        m_camera.transform.parent.eulerAngles = new Vector3(m_camera.transform.parent.eulerAngles.x, yAngle, zAngle);
    }


    public override void SynchronizePos()
    {
        base.SynchronizePos();
        UpdatePos(int.MaxValue, int.MaxValue, int.MaxValue);
    }

}
