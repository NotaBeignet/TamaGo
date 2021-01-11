using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.Assertions;
using System;


public enum SOUND_PACK_ELEMENT_TYPE { INTRO, EXPLOSION, LOOP1, LOOP2, LOOP3 };


public enum RANDOM_SOUND_TYPE { LOOSE, MENU_MUSIC, GAME_MUSIC };

[System.Serializable]
public class SoundPackElement
{
    [SerializeField]
    SOUND_PACK_ELEMENT_TYPE m_type;
    [SerializeField]
    AudioClip m_audioClip;

    public AudioClip AudioClip { get => m_audioClip; set => m_audioClip = value; }
    public SOUND_PACK_ELEMENT_TYPE Type { get => m_type; set => m_type = value; }
}

[System.Serializable]
public class SoundPackInfos
{
    [SerializeField]
    string m_title;

    public string Title { get => m_title; set => m_title = value; }
}


[System.Serializable]
public class SoundPack
{
    [SerializeField]
    List<SoundPackElement> m_listElement;

    [SerializeField]
    SoundPackInfos m_infos;

    AUDIOSOURCE_KEY m_audioSourceKey = AUDIOSOURCE_KEY.CREATE_KEY;

    bool m_isEnable = true;
    
    public bool IsEnable 
    {
        get
        {
            return m_isEnable;
        }

        set
        {
            m_isEnable = value;
            SoundManager.Instance.UpdateSoundPack();
        }
    }

    public AUDIOSOURCE_KEY AudioSourceKey { get => m_audioSourceKey; set => m_audioSourceKey = value; }
    public SoundPackInfos Infos { get => m_infos; set => m_infos = value; }

    public AudioClip GetAudioClip(SOUND_PACK_ELEMENT_TYPE a_elementType)
    {
        AudioClip res = null;
        SoundPackElement element = m_listElement.Find((o) => o.Type == a_elementType);
#if UNITY_EDITOR
        if (element == null)
        {
            Debug.LogError("Element " + a_elementType + " don't exist in the sound pack");
        }
#endif
        res = element.AudioClip;
        return res;
    }

    public void Init()
    {
        foreach(SoundPackElement element in m_listElement)
        {
            element.AudioClip.LoadAudioData();
        }
    }
}

[System.Serializable]
public class RandomSoundPack
{
    [SerializeField]
    RANDOM_SOUND_TYPE m_type;
    [SerializeField]
    List<SoundPack> m_listSoundPacks;
    RandomCycle m_randomCycle;
    [SerializeField]
    AUDIOSOURCE_KEY m_audioSourceKey = AUDIOSOURCE_KEY.CREATE_KEY;

#if UNITY_EDITOR
    [SerializeField]
    int m_playInFirst = -1;
    public int PlayInFirst { get => m_playInFirst; set => m_playInFirst = value; }
#endif

    public void Reset()
    {
        m_randomCycle = null;
    }

    #region GetterSetter

    public RandomCycle RandomCycle
    {
        get
        {
            if(m_randomCycle == null)
            {
                int[] randoms = new int[m_listSoundPacks.FindAll((o) => o.IsEnable).Count];
                int numberAdded = 0;
                for(int i = 0; i < m_listSoundPacks.Count; ++i)
                {
                    if(m_listSoundPacks[i].IsEnable)
                    {
                        randoms[numberAdded] = i;
                        ++numberAdded;
                    }
                }
                m_randomCycle = new RandomCycle(randoms);
            }

            return m_randomCycle;
        }

        private set
        {
            m_randomCycle = value;
        }
    }

    public List<SoundPack> ListSoundPacks
    {
        get
        {
            return m_listSoundPacks;
        }

        private set
        {
            m_listSoundPacks = value;
        }
    }

    public RANDOM_SOUND_TYPE Type
    {
        get
        {
            return m_type;
        }

        private set
        {
            m_type = value;
        }
    }

    public AUDIOSOURCE_KEY AudioSourceKey
    {
        get
        {
            return m_audioSourceKey;
        }

        set
        {
            m_audioSourceKey = value;
            foreach(SoundPack pack in m_listSoundPacks)
            {
                pack.AudioSourceKey = value;
            }
        }
    }
    #endregion
}

public enum MIXER_GROUP_TYPE { AMBIANT, SFX_MENU, SFX_GOOD, SFX_BAD , SFX, MASTER};

[System.Serializable]
public class MixerGroupLink
{
    [SerializeField]
    MIXER_GROUP_TYPE m_mixerType;
    [SerializeField]
    AudioMixerGroup m_mixerGroup;

    [SerializeField]
    string m_volumeVariable;

    public MIXER_GROUP_TYPE MixerType
    {
        get
        {
            return m_mixerType;
        }

        private set
        {
            m_mixerType = value;
        }
    }

    public AudioMixerGroup MixerGroup
    {
        get
        {
            return m_mixerGroup;
        }

        private set
        {
            m_mixerGroup = value;
        }
    }

    public string VolumeVariable
    {
        get
        {
            return m_volumeVariable;
        }
    }
}

[System.Serializable]
public class AudioClipLink
{
    [SerializeField]
    AUDIOCLIP_KEY m_key;
    [SerializeField]
    AudioClip m_audioClip;

    public AUDIOCLIP_KEY Key
    {
        get
        {
            return m_key;
        }
    }

    public AudioClip AudioClip
    {
        get
        {
            return m_audioClip;
        }
    }
}


public class AudioSourceExtend
{
    AudioSource m_audioSource;
    bool m_autoDestroy;
    float m_speed;
    int m_key;
    float m_currentTime;
    float m_wantedVolume;
    Action m_callbackOnDestroy;
    #region GetterSetter
    public bool IsDestroyed
    {
        get
        {
            return m_audioSource == null;
        }
    }

    public AudioSource AudioSource
    {
        get
        {
            return m_audioSource;
        }

        set
        {
            m_audioSource = value;
        }
    }

    public bool AutoDestroy
    {
        get
        {
            return m_autoDestroy;
        }

        set
        {
            m_autoDestroy = value;
        }
    }

    public float Speed
    {
        get
        {
            return m_speed;
        }

        set
        {
            m_currentTime = 0;
            m_speed = value;
        }
    }

    public int Key
    {
        get
        {
            return m_key;
        }

        set
        {
            m_key = value;
        }
    }

    public float WantedVolume { get => m_wantedVolume; set => m_wantedVolume = value; }
    public Action CallbackOnDestroy { get => m_callbackOnDestroy; set => m_callbackOnDestroy = value; }

    #endregion


    public AudioSourceExtend(AudioSource a_audioSource)
    {
        m_audioSource = a_audioSource;
    }

    public void Update()
    {

        if(AudioSource == null)
        {
            //Todo Need to destroy this
            return;
        }

        if(Time.deltaTime == 0)
        {
            m_currentTime += Mathf.Abs(Speed) *  1 / 60f;
        }
        else
        {
            m_currentTime += Mathf.Abs(Speed) * Time.deltaTime;
        }

        m_currentTime += Mathf.Abs(Speed) * Time.deltaTime;
        AudioSource.volume = ConcreteEaseMethods.ExpoEaseOut(m_currentTime, Speed > 0 ? 0 : m_wantedVolume, Speed > 0 ? m_wantedVolume : -m_wantedVolume, 1);
        TryToDestroy();
    }

    void TryToDestroy()
    {
        if ( AudioSource && (AutoDestroy || !AudioSource.loop) && AudioSource.clip != null && (!AudioSource.isPlaying  || Mathf.Approximately(AudioSource.volume, 0)))
        {
            if (CallbackOnDestroy != null)
            {
                CallbackOnDestroy();
            }
            GameObject.DestroyImmediate(AudioSource);

        }
    }

    public void Stop()
    {
        if (m_audioSource)
        {
            m_audioSource.Stop();
        }
    }
}


public enum AUDIOSOURCE_KEY { BACKGROUND, BUTTON_MENU, MAIN_MUSIC, NO_KEY_AUTODESTROY, CREATE_KEY };

public enum AUDIOCLIP_KEY{ BONUS_PICKED, BONUS_USED, ENEMY_DIE, ENEMY_FIRE, HITTED , LOOSE, WIN, ENNEMY_HITTED, BUTTON_MENU, RELOADED, COUNTDOWN_CLASSIC, COUNTDOWN_FINAL, EXPLOSION, POWER_UP , ARMOR_RUSH};


public class SoundManager : Singleton<SoundManager>
{
    /// <summary>
    /// Store mixers of game
    /// </summary>
    [SerializeField]
    List<MixerGroupLink> m_listMixerGroup;

    /// <summary>
    /// Store audioClip
    /// </summary>
    [SerializeField]
    List<AudioClipLink> m_listAudioClip;


    /// <summary>
    /// Store set of random sounds
    /// </summary>
    [SerializeField]
    List<RandomSoundPack> m_listRandomSound;

    /// <summary>
    /// Store Audio Source Extend with their keys
    /// </summary>
    Dictionary<int, AudioSourceExtend> m_audioSourcesExtendWithKey = new Dictionary<int, AudioSourceExtend>();
    /// <summary>
    /// Store all AudioSource Extend know
    /// </summary>
    List<AudioSourceExtend> m_audioSourcesExtend = new List<AudioSourceExtend>();


    /// <summary>
    /// Key to allocate
    /// </summary>
    int m_maxAllocatedKey = (int)AUDIOSOURCE_KEY.CREATE_KEY;

    /// <summary>
    /// Step to fade sounds
    /// </summary>
    [SerializeField]
    float m_speedFade = 0.1f;

    //m_currentSoundPack devrais pas être unique, mais il devrai sy en avoir un par randomsounttype
    SoundPack m_currentSoundPack;

    void Update()
    {

        for(int i = 0; i < m_audioSourcesExtend.Count; ++i)
        {
            AudioSourceExtend audioSourceExtend = m_audioSourcesExtend[i];
            audioSourceExtend.Update();
            if (audioSourceExtend.IsDestroyed)
            {
                if (m_audioSourcesExtendWithKey.ContainsKey(audioSourceExtend.Key) && m_audioSourcesExtendWithKey[audioSourceExtend.Key] == audioSourceExtend)
                {
                    m_audioSourcesExtendWithKey.Remove(audioSourceExtend.Key);
                }
            }
        }
        m_audioSourcesExtend.RemoveAll(x => x.IsDestroyed);
    }

    //todo should be encapsulate
    public void SetArmorRushSound(bool a_bool)
    {
    /*    MixerGroupLink mixer = m_listMixerGroup.Find(x => x.MixerType == MIXER_GROUP_TYPE.AMBIANT);
        Assert.AreNotEqual(mixer, null, "MixerGroup don't exist");

        mixer.MixerGroup.audioMixer.SetFloat("HighPassCutOff", a_bool ? 34 : 10);
        mixer.MixerGroup.audioMixer.SetFloat("HighPassResonance", a_bool ? 6f : 1.0f);
        mixer.MixerGroup.audioMixer.SetFloat("Distorsion", a_bool ? 0.05f : 0);
       // mixer.MixerGroup.audioMixer.SetFloat("Pitch", a_bool ? 1.06f : 1);
        mixer.MixerGroup.audioMixer.SetFloat("FrequencyGain", a_bool ? 1.35f : 1);*/
    }

    public void SetMixerVolume(MIXER_GROUP_TYPE a_mixerGroupType, float a_volume)
    {
        MixerGroupLink mixer = m_listMixerGroup.Find(x => x.MixerType == a_mixerGroupType);
        Assert.AreNotEqual(mixer, null, "MixerGroup don't exist");

        mixer.MixerGroup.audioMixer.SetFloat(mixer.VolumeVariable, a_volume);
    }

    public float GetMixerVolume(MIXER_GROUP_TYPE a_mixerGroupType)
    {
        float res = 0;
        MixerGroupLink mixer = m_listMixerGroup.Find(x => x.MixerType == a_mixerGroupType);
        Assert.AreNotEqual(mixer, null, "MixerGroup don't exist");

         mixer.MixerGroup.audioMixer.GetFloat(mixer.VolumeVariable, out res);
        return res;
    }

    /// <summary>
    /// Check if an Audio is actually playing
    /// </summary>
    /// <param name="a_key">Key of the audio</param>
    /// <returns>Do the audio playing</returns>
    public bool IsAudioPlaying(int a_key)
    {
        bool res = false;
        AudioSourceExtend output;
        if(m_audioSourcesExtendWithKey.TryGetValue(a_key, out output))
        {
            res = output.AudioSource.isPlaying;
        }

        return res;
    }

    /// <summary>
    /// Get audiosourceassociate to a key if it not exist will create a new depend of parameter
    /// </summary>
    /// <param name="a_key">key of the audio</param>
    /// <param name="out_res">out</param>
    /// <param name="a_createIfNotPresent">Do we create a new source if not exist</param>
    /// <returns>True if the key ever exist, false if not</returns>
    bool GetAudioSource(int a_key, out AudioSourceExtend out_res, bool a_createIfNotPresent = true)
    {
        Assert.IsFalse(a_key < 0 || a_key > m_maxAllocatedKey, "Bad sound key");

        bool res = true;
        if (!m_audioSourcesExtendWithKey.TryGetValue(a_key, out out_res))
        {
            res = false;
            if (a_createIfNotPresent)
            {
                out_res = new AudioSourceExtend(CreateAudioSource());
                if (a_key != (int)AUDIOSOURCE_KEY.NO_KEY_AUTODESTROY)
                {
                    m_audioSourcesExtendWithKey.Add(a_key, out_res);
                    out_res.Speed = -m_speedFade;
                    out_res.AutoDestroy = true;
                }
                else
                {
                    out_res.AutoDestroy = false;
                }
                out_res.Key = a_key;
                m_audioSourcesExtend.Add(out_res);
            }

        }

  
        return res;
    }

    /// <summary>
    /// Create an audio source with looping true and not playing
    /// </summary>
    /// <returns>an Audios ource</returns>
    AudioSource CreateAudioSource(GameObject a_parent = null)
    {

        if(a_parent == null)
        {
            a_parent = gameObject;
        }
        AudioSource res;
        res = a_parent.AddComponent<AudioSource>();
        res.loop = true;
        res.Stop();
        return res;
    }


    /// <summary>
    /// depend of the key create a new one or just return this key.
    /// </summary>
    /// <param name="a_key">the key</param>
    /// <returns>key created</returns>
    int GetKey(int a_key)
    {
        Assert.IsFalse(a_key < 0 || a_key > m_maxAllocatedKey, "Bad sound key");
  
        if (a_key == (int)AUDIOSOURCE_KEY.CREATE_KEY)
        {
            a_key = ++m_maxAllocatedKey;
        }
        return a_key;
    }

    /// <summary>
    /// Get a key to keep tracking of audiosource
    /// </summary>
    /// <returns>a key</returns>
    public int GenerateKey()
    {
        return GetKey((int)AUDIOSOURCE_KEY.CREATE_KEY);
    }


    public int StartAudio(AUDIOCLIP_KEY a_clipKey, MIXER_GROUP_TYPE a_mixerGroupType = MIXER_GROUP_TYPE.AMBIANT, bool a_isFading = true, bool a_isLooping = true, AUDIOSOURCE_KEY a_key = AUDIOSOURCE_KEY.CREATE_KEY, ulong a_delay = 0, GameObject a_parent = null, float a_volume = 1.0f, bool a_spatial = true)
    {
        AudioClipLink audioClip = m_listAudioClip.Find(o => o.Key == a_clipKey);

        Assert.IsFalse(audioClip == null, "Bad audioclip key");

        return StartAudio(audioClip.AudioClip,a_mixerGroupType, a_isFading,a_isLooping,a_key,a_delay, a_parent, a_volume, a_spatial);

    }

    //TODO see to move initialisation of audiosource elsewhere - store key inside audio extends instead of dictionnary?
    //TODO : Store a part of audioclips here and associate key
    //todo should move all this parameter inside the parameters. Because a sound is not used with different paramters than define at beginning. Those option should be set by sound and not when playing them

    /// <summary>
    /// Start an audio depending of parameters - TODO simplify it and move elswhere some logic
    /// </summary>
    /// <param name="a_clip">Clip we want to play</param>
    /// <param name="a_mixerGroupType">Mixer we want to output</param>
    /// <param name="a_isFading">If the clip is fading</param>
    /// <param name="a_isLooping">If the clip it autolooping</param>
    /// <param name="a_key">the key we want</param>
    /// <param name="a_delay">if we play with a delay</param>
    /// <returns></returns>
    public int StartAudio(AudioClip a_clip, MIXER_GROUP_TYPE a_mixerGroupType = MIXER_GROUP_TYPE.AMBIANT, bool a_isFading = true, bool a_isLooping = true, AUDIOSOURCE_KEY a_key = AUDIOSOURCE_KEY.CREATE_KEY, ulong a_delay = 0, GameObject a_parent = null, float a_volume = 1.0f, bool a_spatial = true)
     {
        int key = GetKey((int)a_key);
        AudioMixerGroup mixer = null;
        try
        {
            mixer = m_listMixerGroup.Find(x => x.MixerType == a_mixerGroupType).MixerGroup;
            AudioSourceExtend source;

            if (GetAudioSource(key, out source))
            {
                //hack to force restart the same clip
               /* if(source.AudioSource.clip == a_clip)
                {
                    Debug.Log("Clip already play : " + a_clip.name);
                    return key;
                }*/
                AudioSourceExtend new_source = new AudioSourceExtend(CreateAudioSource(a_parent));
                m_audioSourcesExtend.Add(new_source);
                m_audioSourcesExtendWithKey[key] = new_source;
                if (a_isFading)
                {
                    if(m_speedFade <= 0)
                    {
                        Debug.LogError("Sound fading bug speed fade <= 0");
                    }
                    source.Speed = -m_speedFade;
                }
                else
                {
                    source.Stop();
                }
                source.AutoDestroy = true;
               // AudioSourceExtend temp = source;
                source = new_source;
             //   new_source = temp;
            }


            if (a_isFading)
            {

                if (m_speedFade <= 0)
                {
                    Debug.LogError("Sound fading bug speed fade <= 0");
                }
                source.Speed = m_speedFade;
                source.AudioSource.volume = 0;
            }

            if (a_spatial)
            {
                source.AudioSource.spatialBlend = 0.45f;
            }

            source.WantedVolume = a_volume;
            source.AudioSource.outputAudioMixerGroup = mixer;
            source.AudioSource.clip = a_clip;
            source.AudioSource.loop = a_isLooping;
            source.AudioSource.Play(a_delay);

          //  Debug.Log("StartAudio : " + a_clip.name);
        }
        catch
        {
            Debug.LogError("Error when try to launch sound");
            if(mixer == null)
            {
                Debug.LogError("Mixer " + a_mixerGroupType + " doesn't exist");
            }
        }
        return key;
     }

    /// <summary>
    /// Stop the audio specify if exist
    /// </summary>
    /// <param name="a_key">key of the audio we want to stop</param>
    public void StopAudio(int a_key = (int)AUDIOSOURCE_KEY.CREATE_KEY)
    {
        AudioSourceExtend audioSourceExtend;
        if (Instance.m_audioSourcesExtendWithKey.TryGetValue(a_key, out audioSourceExtend))
        {
            audioSourceExtend.Stop();
            Debug.Log("StopAudio : " + a_key);
        }
    }

    public List<SoundPack> GetSoundPacks(RANDOM_SOUND_TYPE a_randomSoundType)
    {
       return  m_listRandomSound.Find(x => x.Type == a_randomSoundType).ListSoundPacks;
    }


    public void SelectRandomSoundPack(RANDOM_SOUND_TYPE a_randomSoundType)
    {
        RandomSoundPack randomSound = m_listRandomSound.Find(x => x.Type == a_randomSoundType);
        randomSound.AudioSourceKey = (AUDIOSOURCE_KEY)GetKey((int)randomSound.AudioSourceKey);
        if (randomSound.RandomCycle.Count > 0)
        {
#if UNITY_EDITOR
            if (randomSound.PlayInFirst >= 0)
            {
                m_currentSoundPack = randomSound.ListSoundPacks[randomSound.PlayInFirst];
            }
            else
#endif
            {
                m_currentSoundPack = randomSound.ListSoundPacks[randomSound.RandomCycle.GetNext()];
            }
            m_currentSoundPack.Init();
        }
        else
        {
            StopAudio((int) randomSound.AudioSourceKey);
            m_currentSoundPack = null;
        }
    }

    /// <summary>
    /// Start an audio of a random bank
    /// </summary>
    /// <param name="a_randomSoundType">id of the random bank</param>
    /// <param name="a_mixerGroupType">id of the mixer we want to output through</param>
    public float StartRandomSoundPack(RANDOM_SOUND_TYPE a_randomSoundType, MIXER_GROUP_TYPE a_mixerGroupType, SOUND_PACK_ELEMENT_TYPE a_elementType, bool a_isFading = true, bool a_isLooping = true, ulong a_delay = 0, GameObject a_parent = null, float a_volume = 1.0f, bool a_spatial = false, float a_offsetFromEnd = 0, Action a_callbackOnDestroy = null)
    {
        float res = 0;

        SelectRandomSoundPack(a_randomSoundType);
        if (m_currentSoundPack != null)
        {
            res = StartInSoundPack(a_mixerGroupType, a_elementType, a_isFading, a_isLooping, a_delay, a_parent, a_volume, a_spatial, false, a_offsetFromEnd, a_callbackOnDestroy);
        }
        else
        {
            m_currentSoundPack = null;
            a_callbackOnDestroy?.Invoke();
        }

        return res;
    }

    public float GetSoundDurationInSoundPack(SOUND_PACK_ELEMENT_TYPE a_elementType)
    {
        float res = 0;

        if (m_currentSoundPack != null)
        {
            res = m_currentSoundPack.GetAudioClip(a_elementType).length;
        }
        return res;
    }

    //TODO: find a way to stop the audio when launch a sound which already exist
    public float StartInSoundPack(MIXER_GROUP_TYPE a_mixerGroupType, SOUND_PACK_ELEMENT_TYPE a_elementType, bool a_isFading = true, bool a_isLooping = true, ulong a_delay = 0, GameObject a_parent = null, float a_volume = 1.0f, bool a_spatial = false, bool a_synchronizedBeginning = false, float a_offsetFromEnd = 0, Action a_callbackOnDestroy = null)
    {
        float res = 0;

        if(m_currentSoundPack == null)
        {
            a_callbackOnDestroy?.Invoke();
            return res;
        }

#if UNITY_EDITOR
        if (m_currentSoundPack.AudioSourceKey == AUDIOSOURCE_KEY.CREATE_KEY)
        {
            Debug.LogError("Try to launch a sound but the pack have never been selected");
        }
#endif
        int key = GetKey((int)m_currentSoundPack.AudioSourceKey);
        AudioSourceExtend currentAudioSource = null;

        float time = 0;
        if (GetAudioSource(key, out currentAudioSource))
        {
            time = currentAudioSource.AudioSource.time;
        }

        AudioClip toPlay = m_currentSoundPack.GetAudioClip(a_elementType);
        StartAudio(toPlay, a_mixerGroupType, a_isFading, a_isLooping, m_currentSoundPack.AudioSourceKey, a_delay, a_parent, a_volume, a_spatial);
        if (GetAudioSource(key, out currentAudioSource))
        {
            if (a_synchronizedBeginning)
            {
                currentAudioSource.AudioSource.time = time;
            }
            else if(a_offsetFromEnd > 0)
            {
                currentAudioSource.AudioSource.time = currentAudioSource.AudioSource.clip.length - a_offsetFromEnd;
            }
            res = currentAudioSource.AudioSource.clip.length - currentAudioSource.AudioSource.time;
            //todo this parametter should be set in start audio directly
            if (a_callbackOnDestroy != null)
            {
                currentAudioSource.CallbackOnDestroy = a_callbackOnDestroy;
            }
        }

        return res;
    }

    /// <summary>
    /// Stop an audio with a key
    /// </summary>
    /// <param name="a_key">key of the audio we want to stop</param>
    public void StopAudioWithFadeOut(int a_key)
    {
        AudioSourceExtend audioSource;
        GetAudioSource(a_key, out audioSource,  false);
        audioSource.Speed = -m_speedFade;
        audioSource.AutoDestroy = true;
    }


    public void UpdateSoundPack()
    {
        foreach(RandomSoundPack randomsoundPack in m_listRandomSound)
        {
            randomsoundPack.Reset();
        }
    }

}