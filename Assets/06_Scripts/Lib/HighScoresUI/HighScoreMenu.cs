using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreMenu : Menu
{
    Transform m_container;

    [SerializeField]
    HighscoreEntry m_highscoreEntryPrefab;

    [SerializeField]
    ScrollRect m_viewerPrefab;

    [SerializeField]
    Button m_resetButton;

    public Transform Container 
    { 
        get
        {
            if(m_container == null)
            {
                m_container = Instantiate(m_viewerPrefab, transform).content;
            }

            return m_container;
        }

        private set => m_container = value; 
    }

    protected override void Start()
    {
        base.Start();

        m_resetButton.onClick.AddListener(ResetScores);
    }

    public void Refresh()
    {
        Utils.DestroyChilds(Container);
        GenerateChilds();
    }

    void GenerateChilds()
    {
        int i = 0;
        List<Highscore> scores = GameManager.SaveManager.Scores;
        foreach (Highscore highscore in scores)
        {
            HighscoreEntry entry = Instantiate(m_highscoreEntryPrefab.gameObject, Container).GetComponent<HighscoreEntry>();
            entry.SetHighScore(highscore);
            if(i%2 == 0)
            {
                entry.SetColored();
            }
            ++i;
        }
    }

    private void ResetScores()
    {
        GameManager.SaveManager.Reset();
        GenerateChilds();
    }

    public override void OnOpen(MENUTYPE a_previousMenu)
    {
        Refresh();
        base.OnOpen(a_previousMenu);
    }


}
