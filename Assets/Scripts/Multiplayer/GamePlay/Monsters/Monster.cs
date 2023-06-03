using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Monster : MonoBehaviour
{
    public PhotonView m_photonView;
    private Animator m_animator;
    private Collider m_collider;
    public PlayField PlayField { get => m_playField; set => m_playField = value; }
    [SerializeField] private PlayField m_playField;

    [SerializeField]private bool m_isMonsterReady = false;

    public bool OnAttack { get => m_onAttack;}
    private bool m_onAttack = false;

    private float m_attackDuration;
    private float m_attackTimer;

    private bool m_isStatInit = false;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_collider = GetComponent<Collider>();
        m_photonView = PhotonView.Get(this);


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

    public void StartAttack(Transform target)
    {
        transform.LookAt(target);
        m_onAttack = true;
        UpdateAnimation();

    }

    private void StopAttack()
    {
        m_attackTimer = m_attackDuration;
        if (PlayField != null) transform.eulerAngles = new Vector3(0, PlayField.transform.eulerAngles.y, 0);
        m_onAttack = false;
        UpdateAnimation();

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

    public void SetUpStats(CardConfig config)
    {
        m_HP = config.HP;
        m_attackDuration = config.attackDuration;
        m_attackTimer = m_attackDuration;

        m_isStatInit = true;
    }

   



    // ==================== PUNRPC ====================
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


    public void SetMonsterReady()
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "SetMonsterReady_RPC");
            m_photonView.RPC("SetMonsterReady_RPC", RpcTarget.AllBuffered);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void SetMonsterReady_RPC()
    {
        m_isMonsterReady = true;
        m_animator.enabled = true;
        m_collider.enabled = true;
    }
}
