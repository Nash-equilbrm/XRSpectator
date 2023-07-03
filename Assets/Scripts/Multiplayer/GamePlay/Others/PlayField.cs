using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayField : MonoBehaviour
{
    public PhotonView photonView;
    public Player m_player;
    public Transform monsterHolder;
    public TMPro.TextMeshPro text;

    public CardDisplayField CurrentCardDisplay { get => m_currentCardDisplay;}
    private CardDisplayField m_currentCardDisplay;



    private void Start()
    {
        transform.SetParent(null);
        FollowTransform followTransform = GetComponent<FollowTransform>();
        if (m_player.PlayerID == 0)
        {
            followTransform.offset.z = -1.2f;
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            followTransform.offset.z = 1.2f;
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

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
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "SetNewMonster_RPC");
            photonView.RPC("SetNewMonster_RPC", RpcTarget.AllBuffered, viewID);
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
 