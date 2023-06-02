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
    public Monster CurrentMonster { get => m_currentMonster; set => m_currentMonster = value; }
    private Monster m_currentMonster = null;


    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if(CurrentMonster == null)
        {
            normalField.SetActive(true);
            monsterField.SetActive(false);
            attackPhaseField.SetActive(false);
        }
        else if(CurrentMonster.OnAttack)
        {
            normalField.SetActive(false);
            monsterField.SetActive(false);
            attackPhaseField.SetActive(true);
        }
        else
        {
            normalField.SetActive(false);
            monsterField.SetActive(true);
            attackPhaseField.SetActive(false);
        }


    }


    public void SetNewMonster(int viewID)
    {
        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "SetNewMonster_RPC");
        photonView.RPC("SetNewMonster_RPC", RpcTarget.AllBuffered, viewID);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    public void SetNewMonster_RPC(int viewID)
    {

        GameObject monster = PhotonView.Find(viewID).gameObject;
        monster.transform.SetParent(transform.Find("Content"));
        monster.GetComponent<Monster>().PlayField = this;
        if (monster)
        {
            CurrentMonster = monster.GetComponent<Monster>();
        }
    }
}
 