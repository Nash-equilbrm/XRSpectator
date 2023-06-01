using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;

public class PlayerDisplayModelState : MyStateMachine
{
    private float m_timer = 2f;
    private GameObject m_selectedPlayField = null;
    private GameObject m_prevHit = null;


    public Player m_player;
    public PlayerDisplayModelState(Player player)
    {
        m_player = player;
    }

    protected override void DoBehavior()
    {
        Debug.Log("DISPLAY_MODEL");

        if (!m_player.IsMyTurn || m_player.OnAttackPhase)
        {
            ExitState = true;
            return;
        }

        m_prevHit = m_selectedPlayField;
        m_selectedPlayField = m_player.GetRayCastHit();


        if(m_selectedPlayField != null && m_selectedPlayField.CompareTag("PlayField"))
        {
            if(m_selectedPlayField != m_prevHit || m_prevHit == null)
            {
                // reset timer
                m_timer = 2f;

                // delete invalid sign or model if remove pointer from previous pointed playfield
                if (m_prevHit != null)
                {
                    m_player.CardChose.Monster.gameObject.SetActive(false);
                    GameManager.Instance.invalidSign.SetActive(false);
                }


                // set new model
                Vector3 position = m_selectedPlayField.transform.position;
                Vector3 rotation = new Vector3(0, m_selectedPlayField.transform.eulerAngles.y, 0);
                // show creatures in card
                if (m_player.MyPlayFields.Contains(m_selectedPlayField))
                {
                    m_player.ShowModel(position, Quaternion.Euler(rotation));
                }
                // invalid action -> show invalid sign
                else
                {
                    m_player.ShowInvalidSign(position, Quaternion.Euler(rotation));
                }
                
            }


            // if holding user's pointer on the playfield owns by player, start timer.
            else if(m_player.MyPlayFields.Contains(m_selectedPlayField))
            {
                if(m_timer <= 0)
                {
                    ExitState = true;
                    return;
                }
                else
                {
                    m_timer -= Time.deltaTime;
                }
            }

            m_prevHit = m_selectedPlayField;
        }
    }

   
    protected override void Exit()
    {
        if (!m_player.IsMyTurn)
        {
            m_player.EndMyTurn();
            m_player.SwitchState(PlayerStateEnum.WAIT);
        }
        else if (m_player.OnAttackPhase)
        {
            m_player.SwitchState(PlayerStateEnum.ATTACK);
        }
        else
        {
            PlayField playField = m_selectedPlayField.GetComponent<PlayField>();
            if (playField)
            {
                playField.PlayFieldCard = m_player.CardChose;
                Monster monster = m_player.CardChose.Monster;
                monster.PlayField = playField;
                monster.SetMonsterReady();
            }
            

            m_player.CardChose.Monster.gameObject.transform.SetParent(m_selectedPlayField.transform.Find("Content"));
            if (!m_player.MyMonsters.Contains(m_player.CardChose.Monster.gameObject))
            {
                m_player.MyMonsters.Add(m_player.CardChose.Monster.gameObject);
            }

            m_player.SwitchState(PlayerStateEnum.CHOOSE_CARD);
        }

        // reset
        m_player.CardChose = null;
        m_timer = 2f;
        m_selectedPlayField = null;
        m_prevHit = null;
    }

    protected override void Initialize()
    {
        StateInitialized = true;
    }

}
