using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTextChange : MonoBehaviour
{
    public Player m_player;
    public TMPro.TMP_Text m_text;

    private struct MenuText
    {
        public string text;
        public Color color;

        public MenuText(string text, Color color)
        {
            this.text = text;
            this.color = color;
        }
    }

    private MenuText m_currentText;


    // texts and colors
    private MenuText youWinText = new MenuText("YOU WIN!", new Color(0, 250, 237));
    private MenuText youLostText = new MenuText("YOU LOSE!", new Color(225, 0, 250));
    private MenuText attackText = new MenuText("Attack your opponent's monster!", new Color(250, 0, 109));
    private MenuText chooseYourMonsterText = new MenuText("Choose your monster!", new Color(0, 250, 250));
    private MenuText waitForYourTurn = new MenuText("Wait for you turn!", Color.white);



    private void Update()
    {
        if (!m_player)
        {
            m_player = GameManager.Instance.playerManager;
            return;
        }

        if(GameManager.Instance.GameResult == GameResultEnum.WIN)
        {
            m_currentText = youWinText;

        }
        else if (GameManager.Instance.GameResult == GameResultEnum.LOSE)
        {
            m_currentText = youLostText;
        }
        else
        {
            if (m_player.IsMyTurn && m_player.CurrentMonster.OnAttack && m_player.CanAttack)
            {
                m_currentText = attackText;
            }
            else if (m_player.IsMyTurn && !m_player.OnAttack)
            {
                m_currentText = chooseYourMonsterText;
            }
            else if (!m_player.IsMyTurn)
            {
                m_currentText = waitForYourTurn;
            }
        }


        m_text.text = m_currentText.text;
        m_text.color = m_currentText.color;
    }


    public IEnumerator WaitForSeconds(float secs)
    {
        yield return new WaitForSeconds(secs);
    }
}
