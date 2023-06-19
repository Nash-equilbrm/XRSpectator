using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    public PlayerStateEnum CurrentStateName = PlayerStateEnum.NONE;

    public int PlayerID { get => m_playerID; }
    [SerializeField] private int m_playerID;


    [SerializeField] private GameObject m_invalidSign = null;
    [SerializeField] private ParticleSystem m_deathEffect = null;

    public PhotonView photonView;
    private MyStateMachine m_currentState;
    private PlayerChooseCardState m_chooseCardState;
    private PlayerDisplayModelState m_displayModelState;
    private PlayerWaitState m_waitState;
    private PlayerAttackState m_attackState;
    private PlayerInitState m_initState;

    public List<int> CardCollection { get => m_cardCollectionIds; }
    [SerializeField] private List<int> m_cardCollectionIds;

    public bool IsReady { get => m_isReady; }
    [SerializeField] private bool m_isReady = false;

    public bool OnAttack { get => m_onAttack; }
    [SerializeField] private bool m_onAttack = false;

    public bool IsMyTurn { get => m_isMyTurn; }
    [SerializeField] private bool m_isMyTurn = false;


    public Player Opponent { get => m_opponent; set => m_opponent = value; }
    [SerializeField] private Player m_opponent;


    public int MonsterChosenID { get => m_monsterChosenID; }
    [SerializeField] private int m_monsterChosenID = -1;

    public int CardChoseIndex { get => m_cardChoseIndex; set => m_cardChoseIndex = value; }
    private int m_cardChoseIndex = -1;


    public List<GameObject> MyPlayFields { get => m_myPlayFields; }
    [SerializeField] private List<GameObject> m_myPlayFields;

    public Dictionary<int, Monster> MyMonsters { get => m_myMonsters; }
    [SerializeField] private Dictionary<int, Monster> m_myMonsters;
    public List<int> DebugMonsterList = new List<int>();



    void Start()
    {
        if (GameManager.Instance.isAudience)
        {
            Destroy(gameObject);
        }
        else
        {
            photonView = GetComponent<PhotonView>();
            m_myPlayFields = new List<GameObject>();
            m_myMonsters = new Dictionary<int, Monster>();

            m_chooseCardState = new PlayerChooseCardState(this);
            m_displayModelState = new PlayerDisplayModelState(this);
            m_waitState = new PlayerWaitState(this);
            m_attackState = new PlayerAttackState(this);
            m_initState = new PlayerInitState(this);

            m_currentState = m_initState;

            if (photonView.IsMine)
            {
                GameObject sign = PhotonNetwork.Instantiate("Prefabs/Menus/" + GameManager.Instance.invalidSignPrefab.name, new Vector3(100, 100, 100), Quaternion.identity);
                GetInvalidSign(sign.GetComponent<PhotonView>().ViewID);

                GameObject deathEffect = PhotonNetwork.Instantiate("Prefabs/Effects/" + GameManager.Instance.deathEffectPrefab.name, new Vector3(100, 100, 100), Quaternion.identity);
                GetDeathEffect(deathEffect.GetComponent<PhotonView>().ViewID);
            }

        }
    }

    void Update()
    {
        if (GameManager.Instance.TrackedWithVuforia && GameManager.Instance.GameResult == GameResultEnum.NONE)
        {
            m_currentState.UpdateState();
        }
    }


    public GameObject GetRayCastHit()
    {
        foreach (var source in MixedRealityToolkit.InputSystem.DetectedInputSources)
        {
            // Ignore anything that is not a hand because we want articulated hands
            if (source.SourceType == Microsoft.MixedReality.Toolkit.Input.InputSourceType.Hand)
            {
                foreach (var p in source.Pointers)
                {
                    if (p is IMixedRealityNearPointer)
                    {
                        // Ignore near pointers, we only want the rays
                        continue;
                    }
                    if (p.Result != null)
                    {
                        var startPoint = p.Position;
                        var endPoint = p.Result.Details.Point;
                        var hitObject = p.Result.Details.Object;
                        if (hitObject)
                        {
                            Debug.Log("HitObject: " + hitObject.name + " type: " + hitObject.tag);
                            return hitObject;
                        }
                    }

                }
            }
        }
        return null;
    }

    public void SwitchState(PlayerStateEnum nextState)
    {
        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "SwitchState_RPC");
        photonView.RPC("SwitchState_RPC", RpcTarget.AllBuffered, nextState);
        PhotonNetwork.SendAllOutgoingCommands();
    }


    [PunRPC]
    private void SwitchState_RPC(PlayerStateEnum nextState)
    {
        switch (nextState)
        {
            case PlayerStateEnum.CHOOSE_CARD:
                {
                    Debug.Log("Player: " + m_playerID + "Switch state " + PlayerStateEnum.CHOOSE_CARD);
                    m_currentState = m_chooseCardState;
                    break;
                }
            case PlayerStateEnum.DISPLAY_MODEL:
                {
                    Debug.Log("Player: " + m_playerID + "Switch state " + PlayerStateEnum.DISPLAY_MODEL);
                    m_currentState = m_displayModelState;
                    break;
                }
            case PlayerStateEnum.ATTACK:
                {
                    Debug.Log("Player: " + m_playerID + "Switch state " + PlayerStateEnum.ATTACK);
                    m_currentState = m_attackState;
                    break;
                }
            case PlayerStateEnum.WAIT:
                {
                    Debug.Log("Player: " + m_playerID + "Switch state " + PlayerStateEnum.WAIT);
                    m_currentState = m_waitState;
                    break;
                }
        }
    }

    public GameObject GetChosenMonsterObject()
    {
        if (MyMonsters.ContainsKey(MonsterChosenID))
        {
            return MyMonsters[MonsterChosenID].gameObject;
        }

        return null;
    }


    // ==================== PUNRPC ====================
    public void SetReady(bool ready)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "SetReady_RPC");
            photonView.RPC("SetReady_RPC", RpcTarget.AllBuffered, ready);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void SetReady_RPC(bool ready)
    {
        m_isReady = ready;
    }

    public void ChooseNewCard(int index)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "ChooseNewCard_RPC");
            photonView.RPC("ChooseNewCard_RPC", RpcTarget.AllBuffered, index);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void ChooseNewCard_RPC(int index)
    {
        m_cardChoseIndex = index;
    }

    public void StartMyTurn(bool startTurn)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "StartMyTurn_RPC");
            photonView.RPC("StartMyTurn_RPC", RpcTarget.AllBuffered, startTurn);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void StartMyTurn_RPC(bool startTurn)
    {
        m_isMyTurn = startTurn;
    }



    public void DoAttack(bool attack)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "DoAttack_RPC");
            photonView.RPC("DoAttack_RPC", RpcTarget.AllBuffered, attack);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void DoAttack_RPC(bool attack)
    {
        m_onAttack = attack;
    }


    public void ShowModel(Vector3 position, Quaternion rotation, bool active)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "ShowModel_RPC");
            photonView.RPC("ShowModel_RPC", RpcTarget.AllBuffered, position, rotation, active);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    private void ShowModel_RPC(Vector3 position, Quaternion rotation, bool active)
    {
        Debug.Log("ShowModel_RPC");
        GameObject model = GetChosenMonsterObject();
        if (!active)
        {
            model.SetActive(false);
            return;
        }
        model.SetActive(true);
        model.transform.position = position;
        model.transform.rotation = rotation;
    }




    public void EndMyTurn()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "EndMyTurn_RPC");
            photonView.RPC("EndMyTurn_RPC", RpcTarget.AllBuffered);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    private void EndMyTurn_RPC()
    {
        m_isMyTurn = false;
        if (Opponent != null)
        {
            Opponent.m_isMyTurn = true;
        }
    }


    public void FindOpponent()
    {
        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "FindOpponent_RPC");
        photonView.RPC("FindOpponent_RPC", RpcTarget.AllBuffered);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    private void FindOpponent_RPC()
    {
        if (Opponent == null)
        {
            gameObject.tag = "Temp";
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj)
            {
                Opponent = obj.GetComponent<Player>();
            }
            gameObject.tag = "Player";
        }
    }

    public void GetPlayFields()
    {
        if (photonView.IsMine)
        {
            Debug.Log("GetPlayFields: player " + PlayerID);

            if (PlayerID == 1)
            {
                PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "GetFirstHalfFields_RPC");
                photonView.RPC("GetFirstHalfFields_RPC", RpcTarget.AllBuffered);
                PhotonNetwork.SendAllOutgoingCommands();

            }
            else if (PlayerID == 2)
            {
                PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "GetSecondHalfFields_RPC");
                photonView.RPC("GetSecondHalfFields_RPC", RpcTarget.AllBuffered);
                PhotonNetwork.SendAllOutgoingCommands();
            }
        }
    }

    [PunRPC]
    private void GetFirstHalfFields_RPC()
    {
        Debug.Log("GetFirstHalfFields_RPC");
        for (int i = 0; i < 5; ++i)
        {
            m_myPlayFields.Add(GameManager.Instance.playFields[i]);
        }
    }

    [PunRPC]
    private void GetSecondHalfFields_RPC()
    {
        Debug.Log("GetSecondHalfFields_RPC");
        for (int i = 0; i < 5; ++i)
        {
            m_myPlayFields.Add(GameManager.Instance.playFields[i + 5]);
        }
    }


    public void ChoseNewMonster(int key)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "ChoseNewMonster_RPC");
            photonView.RPC("ChoseNewMonster_RPC", RpcTarget.AllBuffered, key);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    private void ChoseNewMonster_RPC(int key)
    {
        m_monsterChosenID = key;
    }


    public void AddNewMonster(int monsterViewID)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "AddNewMonster_RPC");
            photonView.RPC("AddNewMonster_RPC", RpcTarget.AllBuffered, monsterViewID);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    private void AddNewMonster_RPC(int monsterViewID)
    {
        GameObject monsterObj = PhotonView.Find(monsterViewID)?.gameObject;
        Monster monster = monsterObj.GetComponent<Monster>();
        m_myMonsters.Add(monsterViewID, monster);
        DebugMonsterList.Add(monsterViewID);
    }

    public void RemoveMonster(int monsterViewID)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "RemoveMonster_RPC");
            photonView.RPC("RemoveMonster_RPC", RpcTarget.AllBuffered, monsterViewID);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    private void RemoveMonster_RPC(int monsterViewID)
    {
        if (m_myMonsters.ContainsKey(monsterViewID))
        {
            m_myMonsters.Remove(monsterViewID);
        }
    }


    public void SetPlayerID(int ID)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "SetPlayerID_RPC");
            photonView.RPC("SetPlayerID_RPC", RpcTarget.AllBuffered, ID);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }
    [PunRPC]
    private void SetPlayerID_RPC(int ID)
    {
        m_playerID = ID;
    }

    private void GetInvalidSign(int viewID)
    {

        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "GetInvalidSign_RPC");
        photonView.RPC("GetInvalidSign_RPC", RpcTarget.AllBuffered, viewID);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    private void GetInvalidSign_RPC(int viewID)
    {
        m_invalidSign = PhotonView.Find(viewID).gameObject;
    }


    public void ShowInvalidSign(Vector3 position, Quaternion rotation, bool active = true)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "ShowInvalidSign_RPC");
            photonView.RPC("ShowInvalidSign_RPC", RpcTarget.AllBuffered, position, rotation, active);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    private void ShowInvalidSign_RPC(Vector3 position, Quaternion rotation, bool active = true)
    {
        if (!active)
        {
            m_invalidSign.SetActive(false);
            return;
        }
        m_invalidSign.SetActive(true);
        m_invalidSign.transform.position = position;
        m_invalidSign.transform.rotation = rotation;
    }



    private void GetDeathEffect(int viewID)
    {

        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "GetDeathEffect_RPC");
        photonView.RPC("GetDeathEffect_RPC", RpcTarget.AllBuffered, viewID);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    private void GetDeathEffect_RPC(int viewID)
    {
        GameObject effectObj = PhotonView.Find(viewID).gameObject;
        if (effectObj)
        {
            m_deathEffect = effectObj.GetComponent<ParticleSystem>();
        }
    }

    public void PlayDeathEffect(Vector3 position)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "PlayDeathEffect_RPC");
            photonView.RPC("PlayDeathEffect_RPC", RpcTarget.AllBuffered, position);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    private void PlayDeathEffect_RPC(Vector3 position)
    {
        m_deathEffect.transform.position = position;
        m_deathEffect.Play();
    }

}

    



public enum PlayerStateEnum
{
    NONE,
    WAIT,
    CHOOSE_CARD,
    DISPLAY_MODEL,
    ATTACK
}