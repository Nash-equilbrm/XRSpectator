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
    public TMPro.TextMeshPro text;

    public Monster CurrentMonster { get => m_currentMonster; }
    [SerializeField] private Monster m_currentMonster = null;


    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if(CurrentMonster == null)
        {
            TurnOnNormalField();
        }
        else if(CurrentMonster.OnAttack)
        {
            TurnOnAttackField();
        }
        else
        {
            TurnOnMonsterField();
        }
        if(m_currentMonster != null)
        {
            text.text = m_currentMonster.Name + ": " + m_currentMonster.CurrentHP;
        }
        else
        {
            text.text = "";
        }
    }
    public void TurnOnNormalField()
    {
        normalField.SetActive(true);
        monsterField.SetActive(false);
        attackPhaseField.SetActive(false);
    }


    public void TurnOnMonsterField()
    {
        normalField.SetActive(false);
        monsterField.SetActive(true);
        attackPhaseField.SetActive(false);
    }

    public void TurnOnAttackField()
    {
        normalField.SetActive(false);
        monsterField.SetActive(false);
        attackPhaseField.SetActive(true);
    }

    // ============== RPC ================


    public void SetNewMonster(int viewID)
    {
        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "SetNewMonster_RPC");
        photonView.RPC("SetNewMonster_RPC", RpcTarget.AllBuffered, viewID);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    public void SetNewMonster_RPC(int viewID)
    {
        if(viewID == -1)
        {
            m_currentMonster = null;
            return;
        }
        GameObject monsterobj = PhotonView.Find(viewID).gameObject;
        monsterobj.transform.SetParent(transform.Find("Content"));
        Monster monster = monsterobj.GetComponent<Monster>();
        if (monster)
        {
            monster.PlayField = this;
            m_currentMonster = monster;
        }
    }
}
 