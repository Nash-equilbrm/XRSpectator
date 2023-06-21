using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayField : MonoBehaviour
{
    public PhotonView photonView;

    public GameObject normalField;
    public GameObject monsterField;
    public GameObject attackPhaseField;
    public Transform monsterHolder;
    public TMPro.TextMeshPro text;

    public CardDisplayField cardDisplayField;

    public void SetCardDisplayField(int viewId)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "SetCardDisplayField_RPC");
            photonView.RPC("SetCardDisplayField_RPC", RpcTarget.AllBuffered, viewId);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void SetCardDisplayField_RPC(int viewId)
    {
        GameObject Obj = PhotonView.Find(viewId).gameObject;
        if (Obj != null)
        {
            cardDisplayField = Obj.GetComponent<CardDisplayField>();
        }
    }

}
 