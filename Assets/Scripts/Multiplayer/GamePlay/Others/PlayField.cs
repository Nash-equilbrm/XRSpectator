using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayField : MonoBehaviour
{
    public PhotonView m_photonView;
    public Player m_player;
    public Transform monsterHolder;
    public TMPro.TextMeshPro text;

    public CardDisplayField CurrentCardDisplay { get => m_currentCardDisplay;}
    [SerializeField] private CardDisplayField m_currentCardDisplay;


    private void Start()
    {
        m_photonView = GetComponent<PhotonView>();
        transform.SetParent(null);
    }

    private void Update()
    {
        transform.eulerAngles = new Vector3(0, (m_player.PlayerID == 0)? 0: 180, 0);

        if (CurrentCardDisplay != null && CurrentCardDisplay.Monster != null)
        {
            text.text = CurrentCardDisplay.Monster.Name + ": " + CurrentCardDisplay.Monster.CurrentHP;
        }
        else
        {
            text.text = "";
        }
    }

    public void SetNewCardDisplay(int viewID)
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "SetNewCardDisplay_RPC");
            m_photonView.RPC("SetNewCardDisplay_RPC", RpcTarget.AllBuffered, viewID);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void SetNewCardDisplay_RPC(int viewID)
    {
        if (viewID != -1)
        {
            GameObject cardDisplayObj = PhotonView.Find(viewID).gameObject;
            CardDisplayField cardDisplay = cardDisplayObj.GetComponent<CardDisplayField>();
            if (cardDisplay)
            {
                m_currentCardDisplay = cardDisplay;
                cardDisplay.Monster.transform.SetParent(monsterHolder);
                cardDisplay.Monster.transform.localPosition = Vector3.zero;
                cardDisplay.Monster.transform.localRotation = Quaternion.identity;
                cardDisplay.Monster.PlayField = this;

            }
            else
            {
                m_currentCardDisplay = null;
            }
        }

    }



    public void OnMonsterDestroy()
    {
        StartCoroutine(OnMonsterDestroyCoroutine());
    }

    private IEnumerator OnMonsterDestroyCoroutine()
    {
        Vector3 cardPos = CurrentCardDisplay.transform.position;
        Vector3 monsterPos = transform.position;

        m_player.CurrentMonster.SetMonsterReady(false);
        m_player.RemoveMonster(m_player.CurrentMonster.m_photonView.ViewID);
        m_player.PlayDeathEffect(monsterPos);
        while (m_player.DeathEffect.isPlaying)
        {
            yield return null;
        }

        CurrentCardDisplay.gameObject.SetActive(false);
        SetNewCardDisplay(-1);

        m_player.PlayDeathEffect(cardPos);
        while (m_player.DeathEffect.isPlaying)
        {
            yield return null;
        }

        m_player.CurrentMonster.PlayField = null;
        m_player.CurrentMonster = null;

    }
}
