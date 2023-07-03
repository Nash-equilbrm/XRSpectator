using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : MyStateMachine
{
    public PlayerStateEnum name = PlayerStateEnum.ATTACK;


    public Player m_player;

    public PlayerAttackState(Player player)
    {
        m_player = player;
    }
    protected override void DoBehavior()
    {
        Debug.Log("ATTACK");
        m_player.CurrentStateName = name;

        if (!m_player.IsMyTurn || m_player.MyMonsters.Count == 0)
        {
            ExitState = true;
            return;
        }

        if(m_player.CurrentMonster != null)
        {
            Monster monster = m_player.CurrentMonster.GetComponent<Monster>();
            Monster target = m_player.Opponent.CurrentMonster.GetComponent<Monster>();

            if (monster)
            {
                if (target)
                {
                    Debug.Log(monster.name + " Attack " + target.name);
                    monster.StartAttack(target.m_photonView.ViewID);
                    ExitState = true;
                }
                else
                {
                    Debug.Log(monster.name + " Attack Player" + m_player.Opponent.PlayerID);
                }
            }
        }
        else
        {
            Debug.Log("No current monster");
        }

    }

    protected override void Exit()
    {
        if (!m_player.IsMyTurn)
        {
            m_player.EndMyTurn();
            m_player.SwitchState(PlayerStateEnum.WAIT);
        }
        else {
            m_player.SwitchState(PlayerStateEnum.DISPLAY_MODEL);
        }
        m_player.DoAttack(false);
    }

    protected override void Initialize()
    {   
        StateInitialized = true;
    }

    private GameObject m_selectedObj = null;
    private GameObject m_prevHitObj = null;
    
    
}
