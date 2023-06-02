using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;

public class PlayerDisplayModelState : MyStateMachine
{
    private float m_timer = 1f;
    private float m_chooseSlotDuration = 1f;
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

        if (!GameManager.Instance.IsMyTurn || GameManager.Instance.OnAttack)
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
                m_timer = m_chooseSlotDuration;

                // delete invalid sign or model if remove pointer from previous pointed playfield
                if (m_prevHit != null)
                {
                    m_player.GetChosenMonster().SetActive(false);
                    m_player.ShowInvalidSign(Vector3.zero, Quaternion.identity ,false);
                }


                // set new model
                Vector3 position = m_selectedPlayField.transform.position;
                Vector3 rotation = new Vector3(0, m_selectedPlayField.transform.eulerAngles.y, 0);
                // show creatures in card
                if (m_player.MyPlayFields.Contains(m_selectedPlayField))
                {
                    m_player.ShowModel(position, Quaternion.Euler(rotation), true);
                }
                // invalid action -> show invalid sign
                else
                {
                    m_player.ShowInvalidSign(position, Quaternion.Euler(rotation), true);
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
        if (!GameManager.Instance.IsMyTurn)
        {
            m_player.EndMyTurn();
            m_player.SwitchState(PlayerStateEnum.WAIT);
        }
        else if (GameManager.Instance.OnAttack)
        {
            m_player.SwitchState(PlayerStateEnum.ATTACK);
        }
        else
        {
            PlayField playField = m_selectedPlayField.GetComponent<PlayField>();
            if (playField)
            {
                Monster monster = m_player.GetChosenMonster().GetComponent<Monster>();
                playField.SetNewMonster(m_player.MonsterChosenID);
                monster.SetMonsterReady();
            }
            
            int monsterViewID = m_player.GetChosenMonster().GetComponent<Monster>().photonView.ViewID;


            m_player.SwitchState(PlayerStateEnum.CHOOSE_CARD);
        }

        // reset
        m_player.ChoseNewMonster(-1);
        m_timer = m_chooseSlotDuration;
        m_selectedPlayField = null;
        m_prevHit = null;
    }

    protected override void Initialize()
    {
        StateInitialized = true;
    }

}
