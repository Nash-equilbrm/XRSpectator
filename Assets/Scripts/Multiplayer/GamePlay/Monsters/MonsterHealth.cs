using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Monster
{
    private int m_HP;
    public int CurrentHP { get => m_currentHP; }
    [SerializeField] private int m_currentHP;




    // ==================== PUNRPC ====================
    private void TakeDamage(int damage)
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "TakeDamage_RPC");
            m_photonView.RPC("TakeDamage_RPC", RpcTarget.AllBuffered, damage);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }


    [PunRPC]
    public void TakeDamage_RPC(int damage)
    {
        m_currentHP -= damage;
        if (m_currentHP <= 0)
        {
            //Die
            m_currentHP = 0;

            m_isMonsterReady = false;
            m_animator.enabled = false;
            m_collider.enabled = false;

            Vector3 pos = transform.position;
            PlayField?.SetNewMonster(-1);
            PlayField = null;

            transform.position = new Vector3(100, 100, 100);
            gameObject.SetActive(false);
            GameManager.Instance.playerManager.PlayDeathEffect(pos);
            GameManager.Instance.playerManager.RemoveMonster(m_photonView.ViewID);

        }
        else if (m_takeDamageEffect != null)
        {
            m_takeDamageEffect?.gameObject.SetActive(true);
            m_takeDamageEffect?.Play();
        }


    }


    
}
