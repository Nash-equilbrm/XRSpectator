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
    [SerializeField] private int m_playerID;
    public GameObject InvalidSign { get => m_invalidSign; set => m_invalidSign = value; }
    [SerializeField] private GameObject m_invalidSign = null;

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
            m_myMonsters = new Dictionary<int, GameObject>();

            m_chooseCardState = new PlayerChooseCardState(this);
            m_displayModelState = new PlayerDisplayModelState(this);
            m_waitState = new PlayerWaitState(this);
            m_attackState = new PlayerAttackState(this);
            m_initState = new PlayerInitState(this);

            m_currentState = m_initState;

            if (photonView.IsMine)
            {
                GameObject sign = PhotonNetwork.Instantiate("Prefabs/Menus/" + GameManager.Instance.invalidSignPrefab.name, new Vector3(100, 100, 100), Quaternion.identity);

                PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "GetInvalidSign_RPC");
                photonView.RPC("GetInvalidSign_RPC", RpcTarget.AllBuffered, sign.GetComponent<PhotonView>().ViewID);
                PhotonNetwork.SendAllOutgoingCommands();
            }

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

    private GameObject currentMonsterChosen = null;
    public GameObject GetChosenMonster()
    {
        if (MyMonsters.ContainsKey(MonsterChosenID))
        {
            return MyMonsters[MonsterChosenID];
        }

        return null;
    }


    // ==================== PUNRPC ====================
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
        GameObject model = GetChosenMonster();
        if (!active)
        {
            model.SetActive(false);
            return;
        }
        model.SetActive(true);
        model.transform.position = position;
        model.transform.rotation = rotation;
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
        if (photonView.IsMine)
        {
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
        m_myMonsters.Add(monsterViewID, monsterObj);
        DebugMonsterList.Add(monsterViewID);
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

    [PunRPC]
    private void GetInvalidSign_RPC(int viewID)
    {
        InvalidSign = PhotonView.Find(viewID).gameObject;
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