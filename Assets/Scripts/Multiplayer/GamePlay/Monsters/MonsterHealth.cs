using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Monster
{
    private int m_HP;
    public int CurrentHP { get => m_currentHP; }
    [SerializeField] private int m_currentHP;


    public void OnMonsterDestroy()
    {
        StartCoroutine(OnMonsterDestroyCoroutine());
    }

    private IEnumerator OnMonsterDestroyCoroutine()
    {
        Vector3 cardPos = m_playField.CurrentCardDisplay.transform.position;
        Vector3 monsterPos = transform.position;
        
        SetMonsterReady(false);
        m_playField.m_player.RemoveMonster(m_photonView.ViewID);
        m_playField.m_player.PlayDeathEffect(monsterPos);
        while (m_playField.m_player.DeathEffect.isPlaying)
        {
            yield return null;
        }

        m_playField.CurrentCardDisplay.gameObject.SetActive(false);
        m_playField.SetNewCardDisplay(-1);

        m_playField.m_player.PlayDeathEffect(cardPos);
        while (m_playField.m_player.DeathEffect.isPlaying)
        {
            yield return null;
        }

        m_playField = null;
    }


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
            OnMonsterDestroy();
        }
        else if (m_takeDamageEffect != null)
        {
            m_takeDamageEffect?.gameObject.SetActive(true);
            m_takeDamageEffect?.Play();
        }


    }


    
}
