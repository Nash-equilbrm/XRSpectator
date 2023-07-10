using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;

public class PlayerDisplayModelState : MyStateMachine
{
    private float m_timer;
    private float m_chooseSlotDuration = 2f;
    private GameObject m_selectedPlayField = null;
    private GameObject m_prevHit = null;

    public Player m_player;

    public PlayerStateEnum name = PlayerStateEnum.DISPLAY_MODEL;
    public PlayerDisplayModelState(Player player)
    {
        m_player = player;
        m_timer = m_chooseSlotDuration;
    }

    protected override void DoBehavior()
    {
        Debug.Log("DISPLAY_MODEL");
        m_player.CurrentStateName = name;

        if (!m_player.IsMyTurn || m_player.OnAttack)
        {
            ExitState = true;
            return;
        }
        if (m_player.m_photonView.IsMine)
        {
            m_prevHit = m_selectedPlayField;
            m_selectedPlayField = m_player.GetRayCastHit();


            if (m_selectedPlayField != null && m_selectedPlayField.CompareTag("CardDisplayField"))
            {
                if (m_selectedPlayField != m_prevHit || m_prevHit == null)
                {
                    // reset timer
                    m_timer = m_chooseSlotDuration;

                    if (m_player.MyCardDisplayFields.Contains(m_selectedPlayField))
                    {
                        m_selectedPlayField.GetComponent<CardDisplayField>().ShowInfo(true);
                    }
                }


                // if holding user's pointer on the playfield owns by player, start timer.
                else if (m_player.MyCardDisplayFields.Contains(m_selectedPlayField))
                {
                    if (m_timer <= 0)
                    {
                        CardDisplayField cardDisplayField = m_selectedPlayField.GetComponent<CardDisplayField>();
                        if (cardDisplayField)
                        {
                            Monster monster = cardDisplayField.Monster;


                            // display the monster and put it on Ready to use
                            if (m_player.m_playField.CurrentCardDisplay != null && monster.m_photonView.ViewID != m_player.m_playField.CurrentCardDisplay.Monster.m_photonView.ViewID)
                            {
                                m_player.m_playField.CurrentCardDisplay.Monster.SetMonsterReady(false);
                            }
                            m_player.m_playField.SetNewCardDisplay(cardDisplayField.m_photonView.ViewID);


                            // set the image for the card display and lift up
                            cardDisplayField.ShowInfo(false);
                            cardDisplayField.LiftUp();

                            monster.SetMonsterReady(true);


                            m_player.UpdateNewMonster();

                            float yAngle = (cardDisplayField.transform.parent.parent.localEulerAngles.y != 0) ? -cardDisplayField.transform.parent.parent.localEulerAngles.y : cardDisplayField.transform.parent.parent.localEulerAngles.z;
                            m_player.cardDisplayMovementControll.Rotate(yAngle);

                            // reset
                            m_timer = m_chooseSlotDuration;
                            m_selectedPlayField = null;
                            m_prevHit = null;
                            return;

                        }
                    }
                    else
                    {
                        m_timer -= Time.deltaTime;
                    }
                }

                m_prevHit = m_selectedPlayField;
            }

            if (m_selectedPlayField != m_prevHit && m_prevHit != null && m_prevHit.CompareTag("CardDisplayField") && m_player.MyCardDisplayFields.Contains(m_prevHit))
            {
                CardDisplayField cardDisplayField = m_prevHit.GetComponent<CardDisplayField>();
                if (cardDisplayField)
                    cardDisplayField.ShowInfo(false);
            }
        }

    }

   
    protected override void Exit()
    {
        if (!m_player.IsMyTurn)
        {
            m_player.EndMyTurn();
            m_player.SwitchState(PlayerStateEnum.WAIT);
        }
        else if (m_player.OnAttack)
        {
            m_player.SwitchState(PlayerStateEnum.ATTACK);
        }
        //else
        //{
        //    m_player.SwitchState(PlayerStateEnum.CHOOSE_CARD);
        //}

        
    }

    protected override void Initialize()
    {
        StateInitialized = true;
    }

}
