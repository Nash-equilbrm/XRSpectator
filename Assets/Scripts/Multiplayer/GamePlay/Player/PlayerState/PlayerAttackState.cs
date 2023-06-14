using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : MyStateMachine
{
    public PlayerStateEnum name = PlayerStateEnum.ATTACK;


    public Player m_player;
    private GameObject m_attacker = null;
    private GameObject m_target = null;

    private float m_getAttackerDuration = 2f;
    private float m_getTargetDuration = 2f;
    private float m_timer = 2f;
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


        if (m_attacker == null)
        {
            SetAttacker();
            if(m_attacker != null && (bool)(m_attacker.GetComponent<Monster>()?.HasAttacked))
            {
                m_attacker = null;
            }
        }
        else if (m_target == null)
        {
            SetTarget();
        }
        else if (m_attacker && m_target)
        {
            m_player.ShowInvalidSign(Vector3.zero, Quaternion.identity, false);

            Monster monster = m_attacker.GetComponent<Monster>();
            Monster target = m_target.GetComponent<Monster>();

            //Monster monsterTarget = m_target.GetComponent<Monster>();
            if (monster)
            {
                Debug.Log(m_attacker.name + " Attack " + m_target.name);
                monster.StartAttack(target.m_photonView.ViewID);
                ExitState = true;
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
        else {
            m_player.SwitchState(PlayerStateEnum.CHOOSE_CARD);
        }
        m_attacker = null;
        m_target = null;
        m_player.DoAttack(false);
    }

    protected override void Initialize()
    {   
        StateInitialized = true;
    }

    private GameObject m_selectedObj = null;
    private GameObject m_prevHitObj = null;
    private void SetTarget()
    {
        Debug.Log("SetTarget");
        m_prevHitObj = m_selectedObj;
        GameObject hit = m_player.GetRayCastHit();

        bool hitCreature = false;

        if (hit && hit.tag.Length >= "Creature".Length && hit.tag.Substring(0, 8) == "Creature")
        {
            if (m_player.PlayerID != int.Parse(hit.tag.Substring(8)))
            {
                m_selectedObj = hit;
                hitCreature = true;
            }

            else
            {
                Vector3 position = hit.transform.position;
                Vector3 rotation = new Vector3(0, hit.transform.eulerAngles.y, 0);
                m_player.ShowInvalidSign(position, Quaternion.Euler(rotation));
            }
        }


        if (hitCreature)
        {
            if (m_selectedObj == m_prevHitObj)
            {
                if (m_timer <= 0f)
                {
                    m_target = m_selectedObj;
                    m_timer = 2f;
                    m_selectedObj = null;
                    m_prevHitObj = null;
                    Debug.Log("Choose Attacker: " + m_attacker.name);
                    return;
                }
                else
                {
                    m_timer -= Time.deltaTime;
                }
            }
            else
            {
                m_timer = 2f;
                m_player.ShowInvalidSign(Vector3.zero, Quaternion.identity, false);
            }
        }
    }

    private void SetAttacker()
    {
        Debug.Log("SetAttacker");
        m_prevHitObj = m_selectedObj;
        GameObject hit = m_player.GetRayCastHit();

        bool hitCreature = false;

        if (hit && hit.tag.Length >= "Creature".Length && hit.tag.Substring(0, 8) == "Creature")
            {
            if (m_player.PlayerID == int.Parse(hit.tag.Substring(8)))
            {
                m_selectedObj = hit;
                hitCreature = true;
            }
           
            else
            {
                Vector3 position = hit.transform.position;
                Vector3 rotation = new Vector3(0, hit.transform.eulerAngles.y, 0);
                m_player.ShowInvalidSign(position, Quaternion.Euler(rotation));
            }
        }


        if (hitCreature)
        {
            if (m_selectedObj == m_prevHitObj)
            {
                if (m_timer <= 0f)
                {
                    m_attacker = m_selectedObj;
                    m_timer = 2f;
                    m_selectedObj = null;
                    m_prevHitObj = null;
                    Debug.Log("Choose Attacker: " + m_attacker.name);
                    return;
                }
                else
                {
                    m_timer -= Time.deltaTime;
                }
            }
            else
            {
                m_timer = 2f;
                m_player.ShowInvalidSign(Vector3.zero, Quaternion.identity, false);
            }
        }
    }
}
