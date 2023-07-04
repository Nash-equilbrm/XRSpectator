using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplayField : MonoBehaviour
{
    public PhotonView m_photonView;
    public Player m_player;

    public Image m_monsterImage;
    public Image m_monsterImageBack;

    public float m_liftDuration;

    public Monster Monster { get => m_Monster; }
    [SerializeField] private Monster m_Monster = null;

    private void Start()
    {
        m_photonView = GetComponent<PhotonView>();
    }

    public void LiftUp()
    {
        if (m_photonView.IsMine)
        {
            StartCoroutine(LiftUpCoRoutine());
        }
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
        if (m_photonView.IsMine)
        {
            StartCoroutine(LiftDownCoRoutine());
        }
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

    public void OnChooseNewMonster()
    {

    }


    //=================RPC=================
    public void SetNewMonster(int viewID)
    {
        PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "SetNewMonster_RPC");
        m_photonView.RPC("SetNewMonster_RPC", RpcTarget.AllBuffered, viewID);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    public void SetNewMonster_RPC(int viewID)
    {
        if (viewID == -1)
        {
            m_Monster = null;
            return;
        }

        GameObject monsterobj = PhotonView.Find(viewID).gameObject;
        Monster monster = monsterobj.GetComponent<Monster>();
        if (monster)
        {
            m_Monster = monster;
        }
    }

    public void ChangeImage(int id)
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "ChangeImage_RPC");
            m_photonView.RPC("ChangeImage_RPC", RpcTarget.AllBuffered, id);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void ChangeImage_RPC(int id)
    {
        m_monsterImage.sprite = m_player.cardConfigs[id].avatarImg;
        //m_monsterImageBack.gameObject.SetActive(true);
        //m_monsterImageBack.sprite = m_player.cardConfigs[id].avatarImg;

    }


    public void SetObjectActive(bool active)
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "SetObjectActive_RPC");
            m_photonView.RPC("SetObjectActive_RPC", RpcTarget.AllBuffered, active);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void SetObjectActive_RPC(bool active)
    {
        gameObject.SetActive(active);
    }


}
