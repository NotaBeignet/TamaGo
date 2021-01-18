using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Region { Region1, COUNT }

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    List<CharacteristicsScriptable> m_listCharacteristics;

    [SerializeField]
    List<Buff> m_listBuffs;

    public CharacteristicsScriptable GetCharacteristic(CHARACTERISTIC a_name)
    {
        return m_listCharacteristics.Find(x => x.Name.Equals(a_name));
    }

    public void Initialize()
    {
        foreach (CharacteristicsScriptable characteristic in m_listCharacteristics)
        {
            characteristic.Initialize();
        }
        foreach(Buff buff in m_listBuffs)
        {
            buff.Start();
        }
    }

    [SerializeField]
    PlayerController m_player;

    [SerializeField]
    Camera25DParam m_cameraParams;

    bool m_isPause;

    public bool IsPause { get => m_isPause || IsGameOver; set => m_isPause = value; }
    public bool RestartSameLevel { get; set; }
    public static SaveManager SaveManager { get => m_saveManager; private set => m_saveManager = value; }
    public bool IsGameOver { get; set; } = true;
    public PlayerController Player { get => m_player; }

    //public const int MaxLevel = 9999;
    static SaveManager m_saveManager;

    int m_currentLevel;

    Timer m_sleepTimer;

    System.Action m_soundCallbackExplosion;

    [SerializeField]
    float m_timeBeforeExplosion;

    [SerializeField]
    float m_percentDelayBeforeBurst = 0.66f;

    [SerializeField]
    float m_timeToKeepscoreDisplay;


#if UNITY_EDITOR
    //[Header("Debug")]
 
#endif


    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        m_saveManager = new SaveManager();
    }

    void Start()
    {
        CameraManager.Instance.CurrentStrategy = new Camera25D(m_cameraParams);
    }

    

    float m_lastTimeScale;
    void Update()
    {
        if(Time.timeScale != m_lastTimeScale && Time.timeScale != 0)
        {
            m_lastTimeScale = Time.timeScale;
            Shader.SetGlobalFloat("_TimeScale", 1.0f / m_lastTimeScale);
        }

      /*  if (Input.GetKeyDown(KeyCode.Space))
        {
            IsPause = !IsPause;
        }*/
    }

    public void GameOver()
    {
        if (!IsGameOver)
        {
            IsGameOver = true;
        }
    }

    public void Sleep(float a_duration)
    {
        IsPause = true;
        m_sleepTimer.FinishTime = a_duration;
        m_sleepTimer.StartTimer();
    }

}
