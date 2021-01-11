using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.Text;


#if UNITY_EDITOR
using UnityEditor;
#endif  //UNITY_EDITOR


public static class Utils
{

    static System.Random m_random = new System.Random();

    public static float Epsilon = 0.05f;

    public static void DestroyChilds(Transform a_transform)
    {
        for (int i = a_transform.childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(a_transform.GetChild(i).gameObject);
        }
    }

    public static void DestroyChildsImmediate(Transform a_transform)
    {
        for (int i = a_transform.childCount - 1; i >= 0; --i)
        {
           GameObject.DestroyImmediate(a_transform.GetChild(i).gameObject);
        }
    }



    public static void RandomizeChildren(Transform a_transform, System.Random r = null)
    {
        if (r == null)
        {
            r = m_random;
        }

        for (int i = 0; i < a_transform.childCount; i++)
        {
            int randomIndex = r.Next(0, a_transform.childCount);
            a_transform.GetChild(i).SetSiblingIndex(randomIndex);
        }
    }

    public static bool RandomBool(float probability, System.Random r = null)
    {
        return RandomFloat(0,100,r) <= probability;
    }


    public static bool RandomBool( int probability, System.Random r = null)
    {
        if (r == null)
        {
            r = m_random;
        }

        int v = r.Next(0, 100);
        bool result = v < probability;
        return result;
    }

    public static float RandomFloat(float min, float max, System.Random r = null)
    {
        if(r == null)
        {
            r = m_random;
        }
        return (float)r.NextDouble() * (max - min) + min;
    }


    public static int RandomInt(int min, int max, System.Random r = null)
    {
        if (r == null)
        {
            r = m_random;
        }
        return r.Next(min, max);
    }


    public static int SignWithZero(float a_value, float a_epsilon = 0)
    {
        return a_value >= -a_epsilon && a_value <= a_epsilon ? 0 : (int)Mathf.Sign(a_value);
    }

    public static bool Equals(float a_value, float a_value2, float a_epsilon)
    {
        return a_value + a_epsilon >= a_value2 && a_value - a_epsilon <= a_value2;
    }


    public static float RandomGaussianDouble(float a_mean, float a_stdDev, System.Random r = null)
    {
        if (r == null)
        {
            r = m_random;
        }


        float u, v, S;
        
        do
        {
            u = 2.0f * (float)r.NextDouble() - 1.0f;
            v = 2.0f * (float)r.NextDouble() - 1.0f;
            S = u * u + v * v;
        } while (S >= 1.0f || S == 0f);

        float fac = Mathf.Sqrt((-2.0f * Mathf.Log(S) / S));
        return a_mean + a_stdDev * u * fac;
    }

    public static float RandomGaussianDoubleMinMax(float a_mean, float a_stdDev, float a_min, float a_max, System.Random r = null)
    {
        float res = RandomGaussianDouble(a_mean, a_stdDev, r);

        res = Mathf.Clamp(res, a_min, a_max);

        return res;
    }


    public static float RandomGaussianDoubleMinMax(float a_stdDev, float a_min, float a_max, System.Random r = null)
    {
        float res = RandomGaussianDouble((a_min + a_max)/2.0f, a_stdDev, r);

        res = Mathf.Clamp(res, a_min, a_max);

        return res;
    }



    public static Vector2 ClampVector(Vector2 a_vector, Vector2 a_vectorMin, Vector2 a_vectorMax)
    {
        a_vector.Normalize();
        a_vectorMin.Normalize();
        a_vectorMax.Normalize();

        Vector2 res = a_vector;
        float current = (Vector2.SignedAngle(new Vector2(1, 0), a_vector) + 360) % 360;
        float negCurrent = current - 360;
        float min = ((Vector2.SignedAngle(new Vector2(1, 0), a_vectorMin) + 360) % 360) - 360;
        float max = (Vector2.SignedAngle(new Vector2(1, 0), a_vectorMax) + 360) % 360;

        if (current > max && negCurrent < min)
        {
            if (current - max < Mathf.Abs(negCurrent - min))
            {
                res = a_vectorMax;
                Debug.Log("Get Max cur " + current + " neg " + negCurrent + " min " + min + " max " + max);

            }
            else

            {

                res = a_vectorMin;
                Debug.Log("Get Min");

            }
        }


        return res;

        //Seem to work only with 0 - 180 ?
        //return Vector2.Min(a_vectorMax, Vector2.Max(a_vectorMin, a_vector));

    }

    public static Vector2 ClampVector(Vector2 a_vector, float a_degreMin, float a_degreMax)
    {
        return ClampVector(a_vector, new Vector2(Mathf.Cos(Mathf.Deg2Rad * a_degreMin), Mathf.Sin(Mathf.Deg2Rad * a_degreMin)), new Vector2(Mathf.Cos(Mathf.Deg2Rad * a_degreMax), Mathf.Sin(Mathf.Deg2Rad * a_degreMax)));
    }

    public static bool IsColorBright(Color color)
    {
        double a = 1 - (0.299 * color.r + 0.587 * color.g + 0.114 * color.b);

        return a < 0.5;
    }



    public static void TriggerNextFrame(Action a_callback)
    {
        MonoBehaviourSingleton.Instance.StartCoroutine(TriggerNextFrameCoroutine(a_callback));
    }

    static IEnumerator TriggerNextFrameCoroutine(Action a_callback)
    {
        yield return new WaitForEndOfFrame();
        a_callback();
    }


    public static void TriggerWaitForSeconds(float a_seconds, Action a_callback)
    {
        MonoBehaviourSingleton.Instance.StartCoroutine(TriggerWaitForSecondsCoroutine(a_seconds, a_callback));
    }

    public static IEnumerator TriggerWaitForSecondsCoroutine(float a_seconds, Action a_callback)
    {
        yield return UtilsYield.GetWaitForSeconds(a_seconds);
        a_callback();
    }


    public static Color ParseColor(string a_hexcolor)
    {
        if (a_hexcolor.StartsWith("#"))
        {
            a_hexcolor = a_hexcolor.Substring(1);
        }

        if (a_hexcolor.StartsWith("0x"))
        {
            a_hexcolor = a_hexcolor.Substring(2);
        }

        if (a_hexcolor.Length != 8)
        {
            throw new Exception(string.Format("{0} is not a valid color string.", a_hexcolor));
        }

        byte r = byte.Parse(a_hexcolor.Substring(0, 2), NumberStyles.HexNumber);
        byte g = byte.Parse(a_hexcolor.Substring(2, 2), NumberStyles.HexNumber);
        byte b = byte.Parse(a_hexcolor.Substring(4, 2), NumberStyles.HexNumber);
        byte a = byte.Parse(a_hexcolor.Substring(6, 2), NumberStyles.HexNumber);

        return new Color32(r, g, b, a);
    }


    public static bool IsTapping(Touch a_touch, int m_minTap = 1)
    {
        return a_touch.tapCount > m_minTap && a_touch.phase == TouchPhase.Began;
    }


    public static void QuitApp()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif //UNITY_EDITOR
    }

    public static bool IsSameSign(int a_number1, int a_number2)
    {
        return (a_number1 ^ a_number2) >= 0;
    }

    public static bool IsSameSign(float a_number1, float a_number2)
    {
        return IsSameSign((int)Mathf.Floor(a_number1), (int)Mathf.Floor(a_number2));
    }


    public static bool IsSameSignWithZero(int a_number1, int a_number2)
    {
        return IsSameSignWithZero((float)a_number1, (float) a_number2);
    }

    public static bool IsSameSignWithZero(float a_number1, float a_number2)
    {
        return SignWithZero(a_number1) == SignWithZero(a_number2);
    }

    public static void ShuffleList(IList a_list)
    {
        var count = a_list.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = a_list[i];
            a_list[i] = a_list[r];
            a_list[r] = tmp;
        }
    }

    public static void ShuffleArray<T>(T[] a_array, int ? a_overrideCount = null)
    {
        int count = a_overrideCount ?? a_array.Length;
        int last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = a_array[i];
            a_array[i] = a_array[r];
            a_array[r] = tmp;
        }
    }


    public static void ShuffleQueue<T>(Queue<T> a_queue)
    {
        List<T> list = new List<T>();
        foreach(T item in a_queue)
        {
            list.Add(item);
        }
        a_queue.Clear();

        ShuffleList(list);

        foreach (T item in list)
        {
            a_queue.Enqueue(item);
        }
    }



    public static float SpringDamper(float a_from, float a_to, float a_time, float a_bounciness)
    {
        float percent = -0.5f * (Mathf.Pow(2.71828f, (-6 * a_time))) * (-2 * Mathf.Pow(2.71828f, (6 * a_time)) + Mathf.Sin(a_bounciness * a_time) + 2 * Mathf.Cos(a_bounciness * a_time));

        return a_from + (percent * (a_to - a_from));
    }


    public static Vector3 SpringDamper(Vector3 a_from, Vector3 a_to, float a_time, float a_bounciness)
    {
        return new Vector3(SpringDamper(a_from.x, a_to.x, a_time, a_bounciness), SpringDamper(a_from.y, a_to.y, a_time, a_bounciness), SpringDamper(a_from.z, a_to.z, a_time, a_bounciness));
    }


    public static float BoundsContainedPercentage(Bounds a_bounds, Bounds a_region)
    {
        float res = 1f;

        for (int i = 0; i < 3; ++i)
        {
            float dist = 0;

            if(a_bounds.min[i] > a_region.center[i])
            {
                dist = a_bounds.max[i] - a_region.max[i];
            }
            else
            {
                dist = a_region.min[i] - a_bounds.min[i];
            }               

            res *= Mathf.Clamp01(1f - dist / a_bounds.size[i]);
        }

        return res;
    }


    public static float GetScaleUIPosY()
    {
        return ((float)Screen.height) / 800.0f;
    }

    public static float GetScaleUIPosX()
    {
        return ((float)Screen.width) / 480.0f;
    }


    public static void Destroy(MonoBehaviour a_mono)
    {
        if(a_mono != null)
        {
            GameObject.Destroy(a_mono.gameObject);
        }
    }


    public static Rect Duplicate(Rect a_rect)
    {
        return new Rect(a_rect.x, a_rect.y, a_rect.width, a_rect.height);
    }


    //helper string
    static StringBuilder m_stringBuilder = new StringBuilder(20, 20);
    static readonly string[] numbers = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
    public static string GenerateStringFromInt(int a_number, string a_suffix = "")
    {
        m_stringBuilder.Clear();
        do
        {
            m_stringBuilder.Insert(0, numbers[a_number % 10], 1);
            a_number /= 10;
        } while (a_number > 0);
        m_stringBuilder.Append(a_suffix);

        return m_stringBuilder.ToString();
    }

    public static ContactPoint FindClosestContact(Collision a_collision)
    {
        ContactPoint res = new ContactPoint();
        float m_mindist = int.MaxValue;
        foreach (ContactPoint point in a_collision.contacts)
        {
            float dist = (a_collision.gameObject.transform.position - point.point).magnitude;
            if (dist < m_mindist)
            {
                m_mindist = dist;
                res = point;
            }
        }

        return res;
    }
}
