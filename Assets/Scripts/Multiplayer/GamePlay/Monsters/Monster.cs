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

    [SerializeField]private bool m_isMonsterReady = false;

    public bool OnAttack { get => m_onAttack;}
    public string Name { get => m_name; }
    private string m_name;

    private bool m_onAttack = false;

    private int m_ATK;
    private float m_attackDuration;
    private float m_attackTimer;

    private bool m_isStatInit = false;

    private void Start()
    {
        m_animator.enabled = false;
        m_collider.enabled = false;

    }

    private void Update()
    {
        if (m_isMonsterReady && m_isStatInit)
        {
            UpdateAttackPhase();
        }

    }

    private void UpdateAnimation()
    {
        m_animator.SetBool("Attacking", OnAttack);
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
            m_photonView.RPC("SetUpStats_RPC", RpcTarget.AllBuffered,config.name, config.HP, config.ATK, config.attackDuration);
            PhotonNetwork.SendAllOutgoingCommands();
            UpdateAnimation();

        }
    }

    [PunRPC]
    public void SetUpStats_RPC(string name, int HP, int ATK, float attackDuration)
    {
        m_name = name;
        m_HP = HP;
        m_currentHP = HP;
        m_ATK = ATK;
        m_attackDuration = attackDuration;
        m_attackTimer = m_attackDuration;

        m_isStatInit = true;
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
    public void StartAttack_RPC(int monsterViewID)
    {
        GameObject targetObj = PhotonView.Find(monsterViewID).gameObject;
        if (targetObj)
        {
            Monster targetMonster = targetObj.GetComponent<Monster>();
            if (targetMonster)
            {
                transform.LookAt(targetObj.transform);
                targetMonster.TakeDamage(m_ATK);
                m_onAttack = true;
            }
        }
    }


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
        if (PlayField != null) transform.eulerAngles = new Vector3(0, PlayField.transform.eulerAngles.y, 0);
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
        m_isMonsterReady = ready;
        m_animator.enabled = ready;
        m_collider.enabled = ready;
    }
}
