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
    private CardDisplayField m_currentCardDisplay;


    private void Start()
    {
        m_photonView = GetComponent<PhotonView>();
        transform.SetParent(null);
        followTransform = GetComponent<FollowTransform>();
    }

    private FollowTransform followTransform;
    private bool m_initRotation = false;

    private void Update()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

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
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "SetNewMonster_RPC");
            m_photonView.RPC("SetNewMonster_RPC", RpcTarget.AllBuffered, viewID);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void SetNewMonster_RPC(int viewID)
    {
        if(viewID != -1)
        {
            GameObject cardDisplayObj = PhotonView.Find(viewID).gameObject;
            CardDisplayField cardDisplay = cardDisplayObj.GetComponent<CardDisplayField>();
            if (cardDisplay) m_currentCardDisplay = cardDisplay;
        }
        else
        {
            m_currentCardDisplay = null;
        }
    }


}
 