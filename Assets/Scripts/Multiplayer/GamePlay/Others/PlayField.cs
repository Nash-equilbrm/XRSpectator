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

    [SerializeField] private Monster m_currentMonster = null;


    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void FixedUpdate()
    {
        if(m_currentMonster == null)
        {
            TurnOnNormalField();
        }
        else if(m_currentMonster.HasAttacked)
        {
            TurnOnAttackField();
        }
        else
        {
            TurnOnMonsterField();
        }

        if (m_currentMonster != null)
        {
            text.text = m_currentMonster.Name + ": " + m_currentMonster.CurrentHP;
            SetNewTag("OccupiedPlayField");
        }
        else
        {
            text.text = "";
            SetNewTag("PlayField");
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
            m_currentMonster?.transform.SetParent(null);
            m_currentMonster = null;
            return;
        }
        GameObject monsterobj = PhotonView.Find(viewID).gameObject;
        monsterobj.transform.SetParent(monsterHolder);
        Monster monster = monsterobj.GetComponent<Monster>();
        if (monster)
        {
            monster.PlayField = this;
            m_currentMonster = monster;
        }
    }

    public void SetNewTag(string tag)
    {
        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "SetNewTag_RPC");
        photonView.RPC("SetNewTag_RPC", RpcTarget.AllBuffered, tag);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    public void SetNewTag_RPC(string tag)
    {
        gameObject.tag = "OccupiedPlayField";
        gameObject.tag = tag;
    }
}
 