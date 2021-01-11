using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighscoreEntry : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    Text m_pseudo;

    [SerializeField]
    Text m_score;

    [SerializeField]
    Text m_index;

    [SerializeField]
    Image m_background;

    [SerializeField]
    Color m_colorEvenText;

    [SerializeField]
    Color m_colorEvenBackGround;

    Highscore m_highscore;

    public void SetHighScore(Highscore a_highscore)
    {
        m_index.text = a_highscore.Level + "."; 
        m_pseudo.text = a_highscore.Pseudo;
        m_score.text = a_highscore.Score + " m";
        m_highscore = a_highscore;
    }

   public void SetColored()
   {
        m_index.color = m_colorEvenText;
        m_pseudo.color = m_colorEvenText;
        m_score.color = m_colorEvenText;
        m_background.color = m_colorEvenBackGround;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       // GameManager.Instance.StartGame(m_highscore.Level, true);
    }
}
