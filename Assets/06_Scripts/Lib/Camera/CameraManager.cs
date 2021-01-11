using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    Stack<CameraStrategy> m_stackCameras = new Stack<CameraStrategy>();

    CameraStrategy m_currentStrategy;
    GameObject m_target;

    Camera m_camera;



    protected Vector3 m_transformationOffset;
    protected Vector3 m_transformationAngle;


    [Header("Shake")]
    [SerializeField]
    float m_maxIntensityShake = 0.3f;
    [SerializeField]
    float m_maxAngleShake = 15f;


    [SerializeField]
    float m_minShakeDecreaseRate = 0.45f;
    [SerializeField]
    float m_minScaleShake = 9;
    [SerializeField]
    float m_minScaleIntensityShake = 1.5f;

    [SerializeField]
    float m_maxShakeDecreaseRate = 0.65f;
    [SerializeField]
    float m_maxScaleShake = 6.5f;
    [SerializeField]
    float m_maxScaleIntensityShake = 2.55f;

    float m_currentShakeValue;

    float m_currentShakeDecreaseRate;
    float m_currentScaleShake;
    float m_currentScaleIntensityShake;

    [Header("Kick")]

    [SerializeField]
    float m_kickBackDuration;

    [SerializeField]
    float m_kickOnDuration;

    Timer m_timerKick;

    bool m_isKickOn;

    Vector3 m_kickDirection;

    float m_kickIntensity;


    [Header("Vertigo")]

    bool m_isVertigo;

    float m_vertigoDeltaFOV;
    float m_vertigoTargetOriginFOV;
    float m_vertigoTargetDistance;

    Timer m_vertigoTimer;

    float m_currentZeroMaxY;

    public CameraStrategy CurrentStrategy
    {
        get => m_currentStrategy;


        set
        {
            if (m_currentStrategy != null)
            {
                m_stackCameras.Push(m_currentStrategy);
            }

            m_currentStrategy = value;
            ActualizeTarget();
        }
    }


    public GameObject Target
    {
        get => m_target;

        set
        {
            m_target = value;
            ActualizeTarget();
        }


    }

    public Camera Camera { get => m_camera; set => m_camera = value; }


    // Start is called before the first frame update
    void Start()
    {
        Camera = GetComponentInChildren<Camera>();

        if (CurrentStrategy == null)
        {
            CurrentStrategy = new CameraFollow(new Vector3(0, 9, -13), new Vector3(33, 0, 0), 4);
        }

        m_timerKick = TimerFactory.Instance.GetTimer();
        m_vertigoTimer = TimerFactory.Instance.GetTimer();
        m_vertigoTimer.Callback = () =>
        {
            m_isVertigo = false;
            Camera.fieldOfView = m_vertigoTargetOriginFOV;
        };
    }

    // Update is called once per frame
    void Update()
    {

        m_camera.transform.localPosition = Vector3.zero;
        m_camera.transform.localEulerAngles = Vector3.zero;
        m_transformationOffset = Vector3.zero;
        m_transformationAngle = Vector3.zero;
        ApplyShake();
        ApplyKick();
        ApplyVertigo();
        m_camera.transform.localPosition += m_transformationOffset;
        m_camera.transform.localEulerAngles += m_transformationAngle;

        m_currentShakeValue -= (Time.deltaTime * m_currentShakeDecreaseRate);
        m_currentShakeValue = Mathf.Clamp01(m_currentShakeValue);

        if (GameManager.Instance.IsPause)
        {
            return;
        }

        if (CurrentStrategy != null)
        {
            CurrentStrategy.Update();
        }

        m_currentZeroMaxY = Camera.ViewportToWorldPoint(Vector3.up).y;
    }

    void ApplyShake()
    {
        float intensity = Mathf.Pow(m_currentShakeValue, m_currentScaleIntensityShake);
        float x = m_maxIntensityShake * intensity * (Mathf.PerlinNoise(0, Time.time * m_currentScaleShake) - 0.5f) * 2;
        float y = m_maxIntensityShake * intensity * (Mathf.PerlinNoise(10, Time.time * m_currentScaleShake) - 0.5f) * 2;
        m_transformationOffset += new Vector3(x, y, 0);

        float angle = m_maxAngleShake * intensity * (Mathf.PerlinNoise(20, Time.time * m_currentScaleShake) - 0.5f) * 2;

        m_transformationAngle += new Vector3(0, 0, angle);

    }


    public void SetShake(float a_shakeValue)
    {
        m_currentShakeValue = a_shakeValue;
        m_currentShakeValue = Mathf.Clamp01(m_currentShakeValue);

        m_currentScaleShake = Mathf.Lerp(m_minScaleShake, m_maxScaleShake, m_currentShakeValue);
        m_currentShakeDecreaseRate = Mathf.Lerp(m_minShakeDecreaseRate, m_maxShakeDecreaseRate, m_currentShakeValue);
        m_currentScaleIntensityShake = Mathf.Lerp(m_maxScaleIntensityShake, m_maxScaleIntensityShake, m_currentShakeValue);
    }


    public void AddShake(float a_shakeValue)
    {
        m_currentShakeValue += a_shakeValue;

        SetShake(m_currentShakeValue);
    }

    void ApplyKick()
    {

        if (m_isKickOn)
        {
            m_transformationOffset += m_kickDirection * Mathf.Lerp(0, m_kickIntensity, m_timerKick.GetPercent());
        }
        else
        {
            m_transformationOffset += m_kickDirection * Mathf.Lerp(m_kickIntensity, 0, m_timerKick.GetPercent());
        }
    }

    void ApplyVertigo()
    {
        if (m_isVertigo)
        {
            m_transformationOffset += Vector3.forward * Mathf.Lerp(0, m_vertigoTargetDistance, m_vertigoTimer.GetPercent());
            Camera.fieldOfView = m_vertigoTargetOriginFOV + Mathf.Lerp(0, m_vertigoDeltaFOV, m_vertigoTimer.GetPercent());
        }
    }

    public void Kick(Vector3 a_direction, float a_intensity)
    {
        m_kickIntensity = a_intensity;
        m_kickDirection = a_direction.normalized;
        m_isKickOn = true;
        m_timerKick.StartTimer(m_kickOnDuration, () =>
        {
            m_isKickOn = false;
            m_timerKick.StartTimer(m_kickBackDuration, null);
        });
    }
    public void Vertigo(float a_time, float a_distance, float a_FOVDelta)
    {
        m_isVertigo = true;
        m_vertigoTimer.FinishTime = a_time;
        m_vertigoTimer.RestartTimer();
        m_vertigoDeltaFOV = a_FOVDelta;
        m_vertigoTargetOriginFOV = Camera.fieldOfView;
        m_vertigoTargetDistance = a_distance;
    }

    public void StopEffects()
    {
        m_currentShakeValue = 0;
    }


    void ActualizeTarget()
    {
        if (CurrentStrategy != null)
        {
            CurrentStrategy.SetTarget(Target);
            //may need to rework stragtegy reference to camera to avoid thisinitialize
            CurrentStrategy.Initialize(Camera);
        }
    }

    public void UnStack()
    {
        if (m_stackCameras.Count > 0)
        {
            CurrentStrategy.Release();
            CurrentStrategy = m_stackCameras.Pop();
        }
    }


    public bool IsVisible(Vector3 a_point)
    {
        Vector3 pos = GetWorldToViewPort(a_point);

        return pos.x >= 0 && pos.x <= 1 && pos.y >= 0 && pos.y <= 1;
    }


    public bool IsXVisible(Vector3 a_point)
    {
        Vector3 pos = GetWorldToViewPort(a_point);
        return pos.x >= 0 && pos.x <= 1;
    }

    public bool IsYVisible(Vector3 a_point)
    {
        Vector3 pos = GetWorldToViewPort(a_point);
        return pos.y >= 0 && pos.y <= 1;
    }

    public bool IsYPercentPassed(Vector3 a_point, float a_percent)
    {
        Vector3 pos = GetWorldToViewPort(a_point);
        return pos.y >= a_percent;
    }

    public bool IsXBetween(Vector3 a_min, Vector3 a_max)
    {
        Vector3 pos1 = GetWorldToViewPort(a_min);
        Vector3 pos2 = GetWorldToViewPort(a_max);

        return (pos2.x >= 0 && pos2.x <= 1) || (pos1.x >= 0 && pos1.x <= 1) || !Utils.IsSameSign(pos1.x, pos2.x);
    }


    public bool IsYBetween(Vector3 a_min, Vector3 a_max)
    {
        Vector3 pos1 = GetWorldToViewPort(a_min);
        Vector3 pos2 = GetWorldToViewPort(a_max);

        return (pos2.y >= 0 && pos2.y <= 1) || (pos1.y >= 0 && pos1.y <= 1) || !Utils.IsSameSign(pos1.y, pos2.y);
    }

    public bool IsYPassed(Vector3 a_pos)
    {
        if(a_pos.z == 0)
        {
            return a_pos.y > m_currentZeroMaxY;
        }

        Vector3 pos1 = GetWorldToViewPort(a_pos);

        return pos1.y > 1;
    }



    Vector3 GetWorldToViewPort(Vector3 a_pos)
    {
        return Camera.WorldToViewportPoint(a_pos);
    }


}