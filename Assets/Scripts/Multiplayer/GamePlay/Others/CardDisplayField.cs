using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplayField : MonoBehaviour
{
    public PhotonView photonView;
    public Player Player { get => m_player; set => m_player = value; }
    private Player m_player;

    public Image m_monsterImage;
    public PlayField m_playField;

    public float m_liftDuration;

    [SerializeField] private Monster m_currentMonster = null;


    private void FixedUpdate()
    {
        if (m_currentMonster != null)
        {
            m_playField.text.text = m_currentMonster.Name + ": " + m_currentMonster.CurrentHP;
        }
        else
        {
            m_playField.text.text = "";
        }
    }

    public void LiftUp()
    {
        StartCoroutine(LiftUpCoRoutine());
    }
    private IEnumerator LiftUpCoRoutine()
    {
        Transform parent = transform.parent;
        float elapsedTime = 0f;
        Quaternion startRotation = parent.localRotation;
        Quaternion targetRotation = Quaternion.Euler(-90f, 0f, 0f);

        while (elapsedTime < m_liftDuration)
        {
            float t = elapsedTime / m_liftDuration;
            parent.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        parent.localRotation = targetRotation;
    }

    public void LiftDown()
    {
        StartCoroutine(LiftDownCoRoutine());
    }
    private IEnumerator LiftDownCoRoutine()
    {
        Transform parent = transform.parent;
        float elapsedTime = 0f;
        Quaternion startRotation = parent.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);

        while (elapsedTime < m_liftDuration)
        {
            float t = elapsedTime / m_liftDuration;
            parent.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        parent.localRotation = targetRotation;
    }
    //=================RPC=================
    public void SetNewMonster(int viewID)
    {
        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "SetNewMonster_RPC");
        photonView.RPC("SetNewMonster_RPC", RpcTarget.AllBuffered, viewID);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    public void SetNewMonster_RPC(int viewID)
    {
        if (viewID == -1)
        {
            m_currentMonster?.transform.SetParent(null);
            m_currentMonster = null;
            tag = "CardDisplayField";
            return;
        }
        GameObject monsterobj = PhotonView.Find(viewID).gameObject;
        monsterobj.transform.SetParent(m_playField.monsterHolder);
        Monster monster = monsterobj.GetComponent<Monster>();
        if (monster)
        {
            monster.PlayField = m_playField;
            m_currentMonster = monster;
        }
        tag = "OccupiedPlayField";
    }

    public void ChangeImage(int id)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "ChangeImage_RPC");
            photonView.RPC("ChangeImage_RPC", RpcTarget.AllBuffered, id);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void ChangeImage_RPC(int id)
    {
        m_monsterImage.sprite = m_player.cardConfigs[id].avatarImg;
    }


    public void SetPlayer(int viewId)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "SetPlayer_RPC");
            photonView.RPC("SetPlayer_RPC", RpcTarget.AllBuffered, viewId);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void SetPlayer_RPC(int viewId)
    {
        GameObject playerObj = PhotonView.Find(viewId).gameObject;
        if(playerObj != null)
        {
            m_player = playerObj.GetComponent<Player>();
        }
    }

}
