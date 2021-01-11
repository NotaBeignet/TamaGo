

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Highscore
{
    int m_level;
    int m_score;
    int m_distance;
    string m_pseudo;

    public int Score { get => m_score; set => m_score = value; }
    public int Distance { get => m_distance; set => m_distance = value; }
    public string Pseudo { get => m_pseudo; set => m_pseudo = value; }
    public int Level { get => m_level; set => m_level = value; }

    public Highscore(int a_level, int a_score, int a_distance, string a_pseudo)
    {
        m_score = a_score;

        m_distance = a_distance;

        if (a_pseudo == "")
        {
            a_pseudo = "NoName";
        }

        m_pseudo = a_pseudo;
        m_level = a_level;
    }

    public Highscore()
    {
    }
}

public class SaveManager
{
    List<int> m_recordedScores = new List<int>();
    string m_recordedScoresRough;
    List<Highscore> m_scores = new List<Highscore>();
    public List<Highscore> Scores { get => m_scores; private set => m_scores = value; }

    bool[] m_unlockedRegion = new bool[(int)Region.COUNT];

    public SaveManager()
    {
        Load();
    }


    void Load()
    {
        LoadHighscores();
        LoadRegions();
    }

    void LoadRegions()
    {
        for(int i = 0; i < m_unlockedRegion.Length; ++i)
        {
            //init save if needed
            if(!PlayerPrefs.HasKey("" + (Region)i))
            {
                PlayerPrefs.SetInt("" + (Region)i, i == 0 ? 1 : 0);
            }

            m_unlockedRegion[i] = PlayerPrefs.GetInt("" + (Region)i) == 1;
        }
    }

    void LoadHighscores()
    {
        m_recordedScoresRough = PlayerPrefs.GetString("recordedScores");
        string[] recordedScoresTab = m_recordedScoresRough.Split(';');
        foreach (string str in recordedScoresTab)
        {
            int record;
            if (int.TryParse(str, out record))
            {
                m_recordedScores.Add(record);
            }
        }

        m_scores.Clear();
        foreach (int i in m_recordedScores)
        {
            Highscore score = new Highscore(i, PlayerPrefs.GetInt("Score" + i), PlayerPrefs.GetInt("Distance" + i), PlayerPrefs.GetString("Pseudo" + i));
            Scores.Add(score);
        }
        Scores.Sort((o, o2) => o.Level.CompareTo(o2.Level));
    }

    public void SaveHighscore(Highscore a_highscore)
    {
        PlayerPrefs.SetInt("Score" + a_highscore.Level, a_highscore.Score);
        PlayerPrefs.SetInt("Distance" + a_highscore.Level, a_highscore.Distance);
        PlayerPrefs.SetString("Pseudo" + a_highscore.Level, a_highscore.Pseudo);

        if (!m_recordedScores.Contains(a_highscore.Level))
        {
            m_recordedScores.Add(a_highscore.Level);
            m_recordedScoresRough += ";" + a_highscore.Level;
            m_scores.Add(a_highscore);
        }
        else
        {
            m_scores.RemoveAll((o) => o.Level == a_highscore.Level);
            m_scores.Add(a_highscore);
        }

        PlayerPrefs.SetString("recordedScores", m_recordedScoresRough);
        PlayerPrefs.Save();
        Scores.Sort((o, o2) => o.Level.CompareTo(o2.Level));
    }

    void SaveRegions()
    {
        for (int i = 0; i < m_unlockedRegion.Length; ++i)
        {
            PlayerPrefs.SetInt("" + (Region)i, m_unlockedRegion[i] ? 1 : 0);
        }
    }

    public bool IsHighScore(int a_score, int a_level)
    {
        bool res = true;

        if (m_recordedScores.Contains(a_level))
        {
            res = m_scores.Find((o) => o.Level == a_level).Score < a_score;
        }

        return res;
    }

    public bool IsHighScore(int a_score)
    {
        return GetHighest().Score < a_score;
    }

    public int GetHighScore(int a_level)
    {
        int res = 0;

        if (m_recordedScores.Contains(a_level))
        {
            res = m_scores.Find((o) => o.Level == a_level).Score;
        }

        return res; 
    }
    public List<Highscore> GetHighScore(Region a_region)
    {
        List<Highscore> res = new List<Highscore>();

        foreach(Highscore score in Scores)
        {
                res.Add(score);
        }
        return res;
    }

    public void Reset()
    {
        PlayerPrefs.SetString("recordedScores", "");

        m_unlockedRegion[0] = true;

        for (int i = 1; i < m_unlockedRegion.Length; ++i)
        {
            m_unlockedRegion[i] = false;
        }
        SaveRegions();

        PlayerPrefs.Save();
        Load();
    }

    public Highscore GetHighest()
    {
        Highscore max = new Highscore();
        if (Scores.Count > 0)
        {
            max = Scores[0];

            foreach(Highscore score in Scores)
            {
                if(score.Score > max.Score)
                {
                    max = score;
                }
            }
        }

        return max;
    }

    public Region GetLastUnlockedRegion()
    {
        for(int i = m_unlockedRegion.Length - 1; i >= 0; --i)
        {
            if(m_unlockedRegion[i])
            {
                return (Region)i;
            }
        }

        Debug.LogError("No Region are unlocked - should never happen");
        return Region.COUNT;
    }

    public bool IsRegionUnlocked(Region a_region)
    {
        if(a_region == Region.COUNT)
        {
            return true;
        }

        return m_unlockedRegion[(int)a_region];
    }

    public void Unlock(Region a_region)
    {
        m_unlockedRegion[(int)a_region] = true;
        SaveRegions();
    }
}
