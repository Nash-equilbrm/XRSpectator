using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Monster : MonoBehaviour
{
    public PhotonView m_photonView;
    public Animator m_animator;
    public Collider m_collider;
    public PlayField PlayField { get => m_playField; set => m_playField = value; }
    [SerializeField] private PlayField m_playField;

    private bool m_isStatInit = false;
    public string Name { get => m_name; }
    private string m_name;


    [SerializeField] private bool m_isMonsterReady = false;

    public bool OnAttack { get => m_onAttack; }

    private bool m_onAttack = false;
    private Monster m_currentTarget = null;

    private int m_ATK;
    private float m_attackDuration;
    private float m_attackTimer;
    public int MonsterID { get => m_monsterID; }
    private int m_monsterID;

    private void Start()
    {
        m_photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (m_isMonsterReady && m_isStatInit)
        {
            UpdateAttackPhase();
        }
    }





    private void UpdateAttackPhase()
    {
        if (OnAttack && m_attackTimer > 0f)
        {
            m_attackTimer -= Time.deltaTime;
        }
        else if (OnAttack && m_attackTimer <= 0f)
        {
            StopAttack();
        }
    }






    // ==================== PUNRPC ====================

    public void SetUpStats(CardConfig config)
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "SetUpStats_RPC");
            m_photonView.RPC("SetUpStats_RPC", RpcTarget.AllBuffered, config.configID, config.monsterName, config.HP, config.ATK, config.attackDuration);
            PhotonNetwork.SendAllOutgoingCommands();
            UpdateAnimation();

        }
    }

    [PunRPC]
    private void SetUpStats_RPC(int id, string name, int HP, int ATK, float attackDuration)
    {
        m_monsterID = id;
        m_name = name;
        m_HP = HP;
        m_currentHP = HP;
        m_ATK = ATK;
        m_attackDuration = attackDuration;
        m_attackTimer = m_attackDuration;
        m_isStatInit = true;
    }


    public void ResetMonsterAttack()
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "ResetMonsterAttack_RPC");
            m_photonView.RPC("ResetMonsterAttack_RPC", RpcTarget.AllBuffered);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    private void ResetMonsterAttack_RPC()
    {
        Debug.Log("Reset attack");
    }


    public void StartAttack(int monsterViewID)
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "StartAttack_RPC");
            m_photonView.RPC("StartAttack_RPC", RpcTarget.AllBuffered, monsterViewID);
            PhotonNetwork.SendAllOutgoingCommands();
            UpdateAnimation();

        }
    }

    [PunRPC]
    private void StartAttack_RPC(int monsterViewID)
    {
        GameObject targetObj = PhotonView.Find(monsterViewID).gameObject;
        if (targetObj)
        {
            transform.LookAt(targetObj.transform);
            Monster targetMonster = targetObj.GetComponent<Monster>();
            if (targetMonster)
            {
                Debug.Log(m_photonView.ViewID + " attack " + targetMonster.m_photonView.ViewID);
                //targetMonster.TakeDamage(m_ATK);
                m_currentTarget = targetMonster;
                m_onAttack = true;
            }

        }
    }

    ParticleSystem.Particle[] p = new ParticleSystem.Particle[200];

    public void StopAttack()
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "StopAttack_RPC");
            m_photonView.RPC("StopAttack_RPC", RpcTarget.AllBuffered);
            PhotonNetwork.SendAllOutgoingCommands();
            UpdateAnimation();
        }
    }



    [PunRPC]
    private void StopAttack_RPC()
    {
        m_attackTimer = m_attackDuration;
        if (PlayField != null) transform.localEulerAngles = Vector3.zero;
        m_currentTarget = null;
        m_onAttack = false;
    }



    public void SetMonsterTag(string tag)
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "SetMonsterTag_RPC");
            m_photonView.RPC("SetMonsterTag_RPC", RpcTarget.AllBuffered, tag);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }


    [PunRPC]
    private void SetMonsterTag_RPC(string tag)
    {
        gameObject.tag = tag;
    }


    public void SetMonsterReady(bool ready)
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "SetMonsterReady_RPC");
            m_photonView.RPC("SetMonsterReady_RPC", RpcTarget.AllBuffered, ready);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void SetMonsterReady_RPC(bool ready)
    {
        if (ready)
        {
            gameObject.SetActive(true);
            m_isMonsterReady = true;
        }
        else
        {
            gameObject.SetActive(false);
            transform.SetParent(null);
            transform.position = new Vector3(100, 100, 100);
            m_isMonsterReady = false;
        }
    }


    public void DealDamage()
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "DealDamage_RPC");
            m_photonView.RPC("DealDamage_RPC", RpcTarget.AllBuffered);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }


    [PunRPC]
    public void DealDamage_RPC()
    {
        if (m_currentTarget && m_currentTarget.CurrentHP > 0)
        {
            m_currentTarget.TakeDamage(m_ATK);
        }
        else if (m_currentTarget && m_currentTarget.CurrentHP <= 0)
        {
            m_currentTarget = null;
        }
        if (m_attackEffect != null)
        {
            m_attackEffect?.gameObject.SetActive(true);
            m_attackEffect?.Play();
            //Debug.Log("Attack particle remains when dealing: " + m_attackEffect.GetParticles(p));
        }
    }
}