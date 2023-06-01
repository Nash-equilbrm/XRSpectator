using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Monster : MonoBehaviour
{
    public PhotonView photonView;
    private Animator m_animator;
    private Collider m_collider;
    public PlayField PlayField { get => m_playField; set => m_playField = value; }

    private PlayField m_playField;

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
        photonView = GetComponent<PhotonView>();

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

    public void SetMonsterReady()
    {
        m_isMonsterReady = true;
        m_animator.enabled = true;
        m_collider.enabled = true;
    }



    // ==================== PUNRPC ====================
    public void SetMonsterTag(string tag)
    {
        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "SetMonsterTag_RPC");
        photonView.RPC("SetMonsterTag_RPC", RpcTarget.AllBuffered, tag);
        PhotonNetwork.SendAllOutgoingCommands();
    }


    [PunRPC]
    private void SetMonsterTag_RPC(string tag)
    {
        gameObject.tag = tag;
    }
}
