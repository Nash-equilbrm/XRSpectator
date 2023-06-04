using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Monster
{
    private int m_HP;
    public int CurrentHP { get => m_currentHP; set => m_currentHP = value; }
    private int m_currentHP;




    // ==================== PUNRPC ====================
    private void TakeDamage(int damage)
    {
        Debug.Log("TAKE DAMAGE");
        PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "TakeDamage_RPC");
        m_photonView.RPC("TakeDamage_RPC", RpcTarget.AllBuffered, damage);
        PhotonNetwork.SendAllOutgoingCommands();
    }


    [PunRPC]
    public void TakeDamage_RPC(int damage)
    {
        m_HP -= damage;
        if (m_HP < 0)
        {
            //Die
            m_HP = 0;

            m_isMonsterReady = false;
            m_animator.enabled = false;
            m_collider.enabled = false;
            
            PlayField.SetNewMonster(-1);
            PlayField = null;

            transform.position = new Vector3(100, 100, 100);
        }


    }


    
}
