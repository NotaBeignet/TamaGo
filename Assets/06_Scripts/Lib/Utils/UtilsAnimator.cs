using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpringDamperUpParam
{
    [SerializeField]
    float m_time;

    [SerializeField]
    float m_finalSize;

    [SerializeField]
    float m_bounciness;

    public float Time { get => m_time; set => m_time = value; }
    public float FinalSize { get => m_finalSize; set => m_finalSize = value; }
    public float Bounciness { get => m_bounciness; set => m_bounciness = value; }
}



public class UtilsAnimator : MonoBehaviour
{

    Transform m_transform;

    //shaking
    bool m_isShaking;
    bool m_isShakingInversed;
    Timer m_timerShake;
    float m_intensityShake;


    //bubble
    bool m_isBubble;
    Timer m_timerBubble;
    float m_intensityBubble;
    float m_bubbleBounciness;
    float m_percentTimeBubbleScaleUp;

    //scaleUp
    bool m_isScaleUp;
    Timer m_timerScaleUp;
    float m_finalSizeScaleUp;
    float m_scaleUpBounciness;


    //scaleUp
    bool m_isScaleDown;
    Timer m_timerScaleDown;
    float m_finalSizeScaleDown;


    Vector3 m_originalPos;
    Vector3 m_originalScale;
    Quaternion m_originalRot;

    public bool IsShaking { get => m_isShaking; private set => m_isShaking = value; }
    public bool IsBubble { get => m_isBubble; private set => m_isBubble = value; }
    public bool IsScaleUp { get => m_isScaleUp; private set => m_isScaleUp = value; }
    public bool IsScaleDown { get => m_isScaleDown; private set => m_isScaleDown = value; }



  /*  [SerializeField]
    bool m_hackAlwaysEnable;*/

    // Start is called before the first frame update
    void Awake()
    {
        m_transform = GetComponent<Transform>();
        m_timerShake = TimerFactory.Instance.GetTimer();
        m_timerBubble = TimerFactory.Instance.GetTimer();
        m_timerScaleUp = TimerFactory.Instance.GetTimer();
        m_timerScaleDown = TimerFactory.Instance.GetTimer();
    }

    public void StopShaking()
    {
        if (IsShaking)
        {
            m_transform.localPosition = m_originalPos;
            m_timerShake.Stop();
            IsShaking = false;
        }

    }

    public void StopScalingUp()
    {
        if (IsScaleUp)
        {
            m_transform.localScale = m_originalScale;
            m_timerScaleUp.Stop();
            IsScaleUp = false;
        }
    }


    public void Shake(float a_time = 0.5f, float a_intensity = 0.5f, bool a_IsInversed = false)
    {
        EndCurrentModifPos();

        if (a_time == 0)
        {
            return;
        }

        m_timerShake.StartTimer(a_time, () => { IsShaking = false; m_transform.localPosition = m_originalPos; });
        IsShaking = true;
        m_isShakingInversed = a_IsInversed;
        m_originalPos = m_transform.localPosition;
        m_intensityShake = a_intensity;
    }

    public void Bubble(float a_time = 0.5f, float a_intensity = 0.5f, float a_bounciness = 5, float a_percentTimeScaleUp = 0.14f, bool a_loop = false)
    {
        EndCurrentModifScale();

        if(a_time == 0)
        {
            return;
        }

        m_timerBubble.StartTimer(a_time, 
            () =>  {
                IsBubble = false;
                m_transform.localScale = m_originalScale;
                if (a_loop)
                {
                    Bubble(a_time, a_intensity, a_bounciness, a_percentTimeScaleUp, a_loop);
                }
            });

        IsBubble = true;
        m_originalScale = m_transform.localScale;
        m_intensityBubble = a_intensity;
        m_percentTimeBubbleScaleUp = a_percentTimeScaleUp;
        m_bubbleBounciness = a_bounciness;
    }

    public void SpringDamperScaleUp(SpringDamperUpParam a_params)
    {
        SpringDamperScaleUp(a_params.Time, a_params.FinalSize, a_params.Bounciness);
    }

    public void SpringDamperScaleUp(float a_time = 0.5f, float a_finalSize = 0.5f, float a_bounciness = 5)
    {
        EndCurrentModifScale();

        if (a_time == 0)
        {
            return;
        }

        m_timerScaleUp.StartTimer(a_time);
        IsScaleUp = true;
        m_finalSizeScaleUp = a_finalSize;
        m_scaleUpBounciness = a_bounciness;
        m_originalScale = transform.localScale;
    }


    public void LinearScaleDown(float a_time = 0.5f, float a_finalSize = 0.0f)
    {
        EndCurrentModifScale();

        if (a_time == 0)
        {
            return;
        }

        m_timerScaleDown.StartTimer(a_time);
        m_originalScale = m_transform.localScale;
        IsScaleDown = true;
        m_finalSizeScaleDown = a_finalSize;
    }


    public void EndCurrentModifScale()
    {
        if(IsBubble)
        {
            m_transform.localScale = m_originalScale;
            m_timerBubble.Stop();
            IsBubble = false;
        }

        if (IsScaleDown)
        {
            m_transform.localScale = new Vector3(m_finalSizeScaleDown, m_finalSizeScaleDown, m_finalSizeScaleDown);
            IsScaleDown = false;
        }

        if(IsScaleUp)
        {
            m_transform.localScale = new Vector3(m_finalSizeScaleUp, m_finalSizeScaleUp, m_finalSizeScaleUp);
            IsScaleUp = false;
        }
    }

    public void EndCurrentModifPos()
    {
        if (IsShaking)
        {
            m_transform.localPosition = m_originalPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsShaking)
        {
            float intensity = 0.0f; 
            if (m_isShakingInversed)
            {
                intensity=Mathf.Lerp(0.0f, m_intensityShake, m_timerShake.GetCurrentTime() / m_timerShake.GetLength());
            }
            else
            {
                intensity=Mathf.Lerp(m_intensityShake, 0.0f, m_timerShake.GetCurrentTime() / m_timerShake.GetLength());
            }
            m_transform.localPosition = m_originalPos + new Vector3(Utils.RandomFloat(-intensity, intensity), Utils.RandomFloat(-intensity, intensity), Utils.RandomFloat(-intensity, intensity));
        }

        //TODO => transform bubble in linear scaleup and springdamper scaledown
        if (IsBubble)
        {
            float percentTime = m_timerBubble.GetLength() * m_percentTimeBubbleScaleUp;

            if (m_timerBubble.GetCurrentTime() < percentTime)
            {
                m_transform.localScale = Vector3.Lerp(m_originalScale, m_originalScale + new Vector3(m_intensityBubble, m_intensityBubble, m_intensityBubble), m_timerBubble.GetCurrentTime() / percentTime);
            }
            else
            {
                m_transform.localScale = Utils.SpringDamper(m_originalScale + new Vector3(m_intensityBubble, m_intensityBubble, m_intensityBubble), m_originalScale
                    , (m_timerBubble.GetCurrentTime() - percentTime) / (m_timerBubble.GetLength() - percentTime), m_bubbleBounciness);
            }
        }


        if (IsScaleUp)
        {
            m_transform.localScale = Utils.SpringDamper(m_originalScale, new Vector3(m_finalSizeScaleUp, m_finalSizeScaleUp, m_finalSizeScaleUp)
                  , m_timerScaleUp.GetCurrentTime() / m_timerScaleUp.GetLength(), m_scaleUpBounciness);

            if (!m_timerScaleUp.IsTimerRunning())
            {
                m_transform.localScale = new Vector3(m_finalSizeScaleUp, m_finalSizeScaleUp, m_finalSizeScaleUp);
                IsScaleUp = false;
            }
        }


        if (IsScaleDown)
        {
            m_transform.localScale = Vector3.Lerp(m_originalScale, new Vector3(m_finalSizeScaleDown, m_finalSizeScaleDown, m_finalSizeScaleDown), m_timerScaleDown.GetCurrentTime() / m_timerScaleDown.GetLength());
            if(!m_timerScaleDown.IsTimerRunning())
            {
                m_transform.localScale = new Vector3(m_finalSizeScaleDown, m_finalSizeScaleDown, m_finalSizeScaleDown);
                IsScaleDown = false;
            }
        }

    }

}
