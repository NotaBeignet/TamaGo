using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VibrationManager
{
    // Vibrator References
    static AndroidJavaObject m_vibrator = null;
    static AndroidJavaClass m_vibrationEffectClass = null;
    static int m_defaultAmplitude = 255;

    // Api Level
    static int m_ApiLevel = 1;
    static bool DoesSupportVibrationEffect() => m_ApiLevel >= 26;    // available only from Api >= 26
    static bool DoesSupportPredefinedEffect() => m_ApiLevel >= 29;   // available only from Api >= 29

    static bool m_isInitialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static bool Initialize()
    {
        if(m_isInitialized)
        {
            return true;
        }

         // load references safely
        if (Application.platform == RuntimePlatform.Android)
        {
            // Get Api Level
            using (AndroidJavaClass androidVersionClass = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                m_ApiLevel = androidVersionClass.GetStatic<int>("SDK_INT");

                // Get UnityPlayer and CurrentActivity
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        m_vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

                        // if device supports vibration effects, get corresponding class
                        if (DoesSupportVibrationEffect())
                        {
                            m_vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                            m_defaultAmplitude = Mathf.Clamp(m_vibrationEffectClass.GetStatic<int>("DEFAULT_AMPLITUDE"), 1, 255);
                        }

                        // if device supports predefined effects, get their IDs
                        if (DoesSupportPredefinedEffect())
                        {
                            PredefinedEffect.EFFECT_CLICK = m_vibrationEffectClass.GetStatic<int>("EFFECT_CLICK");
                            PredefinedEffect.EFFECT_DOUBLE_CLICK = m_vibrationEffectClass.GetStatic<int>("EFFECT_DOUBLE_CLICK");
                            PredefinedEffect.EFFECT_HEAVY_CLICK = m_vibrationEffectClass.GetStatic<int>("EFFECT_HEAVY_CLICK");
                            PredefinedEffect.EFFECT_TICK = m_vibrationEffectClass.GetStatic<int>("EFFECT_TICK");
                        }
                    }
                }
            }

            Debug.Log("[VibrationManager] Vibration component initialized");
            m_isInitialized = true;
        }

        return m_isInitialized && HasVibrator();
    }

    /// <summary>
    /// Vibrate for Milliseconds, with Amplitude (if available).
    /// If amplitude is -1, amplitude is Disabled. If -1, device DefaultAmplitude is used. Otherwise, values between 1-255 are allowed.
    /// If 'cancel' is true, Cancel() will be called automatically.
    /// </summary>
    public static void Vibrate(long a_milliseconds, int a_amplitude = -1, bool a_cancel = true)
    {
        if (!Initialize())
        {
            Debug.Log("[VibrationManager] Not initialized correctly");
            Handheld.Vibrate();
            return;
        }


        if (a_cancel)
        {
            Cancel();
        }

        if (DoesSupportVibrationEffect())
        {
            // if amplitude is not supported, use 255; if amplitude is -1, use systems DefaultAmplitude. Otherwise use user-defined value.
            a_amplitude = CheckAmplitude(a_amplitude);
            VibrateEffect(a_milliseconds, a_amplitude);
        }
        else
        {
            vibrateLegacy(a_milliseconds);
        }   
    }
    /// <summary>
    /// Vibrate Pattern (pattern of durations, with format Off-On-Off-On and so on).
    /// Amplitudes can be Null (for default) or array of Pattern array length with values between 1-255.
    /// To cause the pattern to repeat, pass the index into the pattern array at which to start the repeat, or -1 to disable repeating.
    /// If 'cancel' is true, Cancel() will be called automatically.
    /// </summary>
    public static void Vibrate(long[] a_pattern, int[] a_amplitudes = null, int a_repeat = -1, bool a_cancel = true)
    {
        if (!Initialize())
        {
            Debug.Log("[VibrationManager] Not initialized correctly");
            return;
        }

        // check Amplitudes array length
        if (a_amplitudes != null && (a_amplitudes.Length != a_pattern.Length || !HasAmplitudeControl()))
        {
            Debug.Log("[VibrationManager] Length of Amplitudes array is not equal to Pattern array OR Device don't support amplitude control. Amplitudes will be ignored.");
            a_amplitudes = null;
        }


        if (a_amplitudes != null)
        {
            for (int i = 0; i < a_amplitudes.Length; i++)
            {
                a_amplitudes[i] = CheckAmplitude(a_amplitudes[i]);
            }
        }

        // vibrate
        if (a_cancel)
        {
            Cancel();
        }

        if (DoesSupportVibrationEffect())
        {
            if (a_amplitudes != null)
            {
                VibrateEffect(a_pattern, a_amplitudes, a_repeat);
            }
            else
            {
                VibrateEffect(a_pattern, a_repeat);
            }
        }
        else
        {
            VibrateLegacy(a_pattern, a_repeat);
        }
    }

    static int CheckAmplitude(int a_amplitude)
    {
        a_amplitude = Mathf.Clamp(a_amplitude, -1, 255);

        if (a_amplitude <= 0)
        {
            a_amplitude = m_defaultAmplitude;
        }
        return a_amplitude;
    }


    /// <summary>
    /// Vibrate predefined effect (described in Vibration.PredefinedEffect). Available from Api Level >= 29.
    /// If 'cancel' is true, Cancel() will be called automatically.
    /// </summary>
    public static void VibratePredefined(PredefinedEffect a_effectId, bool a_cancel = true)
    {
        if (!Initialize())
        {
            Debug.Log("[VibrationManager] Not initialized correctly");
            return;
        }

        if (DoesSupportPredefinedEffect() == false)
        {
            Debug.Log("[VibrationManager] Device doesn't support Predefined Effects (Api Level >= 29)");
            return;
        }


        if (a_cancel)
        {
            Cancel();
        }

        VibrateEffectPredefined(a_effectId);
    }


    /// <summary>
    /// Returns true if device has vibrator
    /// </summary>
    public static bool HasVibrator()
    {
        return m_vibrator != null && m_vibrator.Call<bool>("hasVibrator");
    }

    /// <summary>
    /// Return true if device supports amplitude control
    /// </summary>
    public static bool HasAmplitudeControl()
    {
        if (HasVibrator() && DoesSupportVibrationEffect())
        {
            return m_vibrator.Call<bool>("hasAmplitudeControl"); // API 26+ specific
        }
        else
        {
            return false; // no amplitude control below API level 26
        }
    }

    /// <summary>
    /// Tries to cancel current vibration
    /// </summary>
    public static void Cancel()
    {
        if (HasVibrator())
        {
            m_vibrator.Call("cancel");
        }
    }



    private static void VibrateEffect(long milliseconds, int amplitude)
    {
        using (AndroidJavaObject effect = CreateEffect_OneShot(milliseconds, amplitude))
        {
            m_vibrator.Call("vibrate", effect);
        }
    }
    private static void vibrateLegacy(long milliseconds)
    {
        m_vibrator.Call("vibrate", milliseconds);
    }

    private static void VibrateEffect(long[] pattern, int repeat)
    {
        using (AndroidJavaObject effect = CreateEffect_Waveform(pattern, repeat))
        {
            m_vibrator.Call("vibrate", effect);
        }
    }
    private static void VibrateLegacy(long[] pattern, int repeat)
    {
        m_vibrator.Call("vibrate", pattern, repeat);
    }

    private static void VibrateEffect(long[] pattern, int[] amplitudes, int repeat)
    {
        using (AndroidJavaObject effect = CreateEffect_Waveform(pattern, amplitudes, repeat))
        {
            m_vibrator.Call("vibrate", effect);
        }
    }
    private static void VibrateEffectPredefined(PredefinedEffect effectId)
    {
        using (AndroidJavaObject effect = CreateEffect_Predefined(effectId))
        {
            m_vibrator.Call("vibrate", effect);
        }
    }


    /// <summary>
    /// Wrapper for public static VibrationEffect createOneShot (long milliseconds, int amplitude). API >= 26
    /// </summary>
    private static AndroidJavaObject CreateEffect_OneShot(long a_milliseconds, int a_amplitude)
    {
        return m_vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", a_milliseconds, a_amplitude);
    }
    /// <summary>
    /// Wrapper for public static VibrationEffect createPredefined (int effectId). API >= 29
    /// </summary>
    private static AndroidJavaObject CreateEffect_Predefined(PredefinedEffect a_effectId)
    {
        return m_vibrationEffectClass.CallStatic<AndroidJavaObject>("createPredefined", a_effectId);
    }
    /// <summary>
    /// Wrapper for public static VibrationEffect createWaveform (long[] timings, int[] amplitudes, int repeat). API >= 26
    /// </summary>
    private static AndroidJavaObject CreateEffect_Waveform(long[] a_timings, int[] a_amplitudes, int a_repeat)
    {
        return m_vibrationEffectClass.CallStatic<AndroidJavaObject>("createWaveform", a_timings, a_amplitudes, a_repeat);
    }
    /// <summary>
    /// Wrapper for public static VibrationEffect createWaveform (long[] timings, int repeat). API >= 26
    /// </summary>
    private static AndroidJavaObject CreateEffect_Waveform(long[] a_timings, int a_repeat)
    {
        return m_vibrationEffectClass.CallStatic<AndroidJavaObject>("createWaveform", a_timings, a_repeat);
    }


    public class PredefinedEffect
    {
        public static int EFFECT_CLICK;         // public static final int EFFECT_CLICK
        public static int EFFECT_DOUBLE_CLICK;  // public static final int EFFECT_DOUBLE_CLICK
        public static int EFFECT_HEAVY_CLICK;   // public static final int EFFECT_HEAVY_CLICK
        public static int EFFECT_TICK;          // public static final int EFFECT_TICK
    }
}