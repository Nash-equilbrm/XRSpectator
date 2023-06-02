using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    public int PlayerID { get => m_playerID; }
    private int m_playerID;

    public PhotonView photonView;
    private MyStateMachine m_currentState;
    private PlayerChooseCardState m_chooseCardState;
    private PlayerDisplayModelState m_displayModelState;
    private PlayerWaitState m_waitState;
    private PlayerAttackState m_attackState;
    private PlayerInitState m_initState;

    public int[] cardCollectionIds;


    public Player Opponent { get => m_opponent; set => m_opponent = value; }
    [SerializeField] private Player m_opponent;

    public bool OpponentEndTurn { get => m_opponentEndTurn; set => m_opponentEndTurn = value; }
    [SerializeField] private bool m_opponentEndTurn = false;
    


    public int MonsterChosenID { get => m_monsterChosenID; }
    [SerializeField] private int m_monsterChosenID = -1;

    public List<GameObject> MyPlayFields { get => m_myPlayFields; }
    [SerializeField] private List<GameObject> m_myPlayFields;

    public Dictionary<int, GameObject> MyMonsters { get => m_myMonsters; }

    [SerializeField] Dictionary<int, GameObject> m_myMonsters;
    public int MyMonsterCounts;

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
            m_myMonsters = new Dictionary<int, GameObject>();

            m_chooseCardState = new PlayerChooseCardState(this);
            m_displayModelState = new PlayerDisplayModelState(this);
            m_waitState = new PlayerWaitState(this);
            m_attackState = new PlayerAttackState(this);
            m_initState = new PlayerInitState(this);

            m_currentState = m_initState;
        }
    }

    void Update()
    {
        if (GameManager.Instance.TrackedWithVuforia)
        {
            UpdatePlayer();
        }
        MyMonsterCounts = m_myMonsters.Count;
    }


    private void UpdatePlayer()
    {
        Debug.Log("UpdatePlayer");

        m_currentState.UpdateState();
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
        switch (nextState)
        {
            case PlayerStateEnum.CHOOSE_CARD:
                {
                    m_currentState = m_chooseCardState;
                    break;
                }
            case PlayerStateEnum.DISPLAY_MODEL:
                {
                    m_currentState = m_displayModelState;
                    break;
                }
            case PlayerStateEnum.ATTACK:
                {
                    m_currentState = m_attackState;
                    break;
                }
            case PlayerStateEnum.WAIT:
                {
                    m_currentState = m_waitState;
                    break;
                }
        }
    }

    private GameObject currentMonsterChosen;
    public GameObject GetChosenMonster()
    {
        if (MonsterChosenID <= 0) return null;
        if (MyMonsters.ContainsKey(MonsterChosenID))
        {
            return MyMonsters[MonsterChosenID];
        }
        else if(currentMonsterChosen == null)
        {
            currentMonsterChosen = PhotonView.Find(MonsterChosenID).gameObject;
        }
        return currentMonsterChosen;

    }


    // ==================== PUNRPC ====================
    public void ShowModel(Vector3 position, Quaternion rotation)
    {
        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "ShowModel_RPC");
        photonView.RPC("ShowModel_RPC", RpcTarget.AllBuffered, position, rotation);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    private void ShowModel_RPC(Vector3 position, Quaternion rotation)
    {
        Debug.Log("ShowModel_RPC");
        GameObject model = GetChosenMonster();
        if (!model.activeSelf)
        {
            model.SetActive(true);
        }
        model.transform.position = position;
        model.transform.rotation = rotation;
    }


    public void ShowInvalidSign(Vector3 position, Quaternion rotation)
    {
        Debug.Log("ShowInvalidSign");
        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "ShowInvalidSign_RPC");
        photonView.RPC("ShowInvalidSign_RPC", RpcTarget.AllBuffered, position, rotation);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    private void ShowInvalidSign_RPC(Vector3 position, Quaternion rotation)
    {
        if (!GameManager.Instance.invalidSign.activeSelf)
        {
            GameManager.Instance.invalidSign.SetActive(true);
        }
        GameManager.Instance.invalidSign.transform.position = position;
        GameManager.Instance.invalidSign.transform.rotation = rotation;
    }



    public void EndMyTurn()
    {
        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "EndMyTurn_RPC");
        photonView.RPC("EndMyTurn_RPC", RpcTarget.AllBuffered);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    private void EndMyTurn_RPC()
    {
        GameManager.Instance.IsMyTurn = false;
        OpponentEndTurn = false;
        if (Opponent != null)
        {
            Opponent.OpponentEndTurn = true;
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
        if (GameManager.Instance.playerManager.PlayerID == 1)
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "GetFirstHalfFields_RPC");
            photonView.RPC("GetFirstHalfFields_RPC", RpcTarget.AllBuffered);
            PhotonNetwork.SendAllOutgoingCommands();

        }
        else
        {
            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "GetSecondHalfFields_RPC");
            photonView.RPC("GetSecondHalfFields_RPC", RpcTarget.AllBuffered);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    private void GetFirstHalfFields_RPC()
    {
        for (int i = 0; i < 5; ++i)
        {
            m_myPlayFields.Add(GameManager.Instance.playFields[i]);
        }
    }

    [PunRPC]
    private void GetSecondHalfFields_RPC()
    {
        for (int i = 0; i < 5; ++i)
        {
            m_myPlayFields.Add(GameManager.Instance.playFields[i + 5]);
        }
    }


    public void ChoseNewMonster(int key)
    {
        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "ChoseNewMonster_RPC");
        photonView.RPC("ChoseNewMonster_RPC", RpcTarget.AllBuffered, key);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    private void ChoseNewMonster_RPC(int key)
    {
        m_monsterChosenID = key;
    }


    public void AddNewMonster(int monsterViewID)
    {
        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "AddNewMonster_RPC");
        photonView.RPC("AddNewMonster_RPC", RpcTarget.AllBuffered, monsterViewID);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    private void AddNewMonster_RPC(int monsterViewID)
    {
        GameObject monsterObj = PhotonView.Find(monsterViewID)?.gameObject;
        m_myMonsters.Add(monsterViewID, monsterObj);
    }


    public void SetPlayerID(int ID)
    {
        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "SetPlayerID_RPC");
        photonView.RPC("SetPlayerID_RPC", RpcTarget.AllBuffered, ID);
        PhotonNetwork.SendAllOutgoingCommands();
    }
    [PunRPC]
    private void SetPlayerID_RPC(int ID)
    {
        m_playerID = ID;
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