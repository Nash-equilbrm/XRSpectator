using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    public PhotonView photonView;

    [Header("Ingame GUI")]
    public GameObject cardPrefab;
    public GameObject playMenuObj;
    public GameObject startGameBtn;
    public GameObject endTurnBtn;
    public GameObject attackBtn;
    public Transform[] cardMenuSlots;

    public CardConfig[] cardConfigs;




    private MyStateMachine m_currentState;
    private PlayerChooseCardState m_chooseCardState;
    private PlayerDisplayModelState m_displayModelState;
    private PlayerWaitState m_waitState;
    private PlayerAttackState m_attackState;
    private PlayerInitState m_initState;


    public Player Opponent { get => m_opponent; set => m_opponent = value; }
    [SerializeField] private Player m_opponent;

    public bool IsMyTurn { get => m_isMyTurn; set => m_isMyTurn = value; }
    [SerializeField] private bool m_isMyTurn = false;

    public bool OnAttackPhase { get => m_onAttackPhase; set => m_onAttackPhase = value; }
    private bool m_onAttackPhase = false;
    
    public Card CardChose { get => m_cardChose; set => m_cardChose = value; }
    private Card m_cardChose = null;

    public bool PlayerReady { get => playerReady; set => playerReady = value; }
    private bool playerReady = false;

    public List<GameObject> MyPlayFields { get => m_myPlayFields; set => m_myPlayFields = value; }
    [SerializeField] private List<GameObject> m_myPlayFields;

    public List<GameObject> MyMonsters { get => m_myMonsters; set => m_myMonsters = value; }
    [SerializeField] private List<GameObject> m_myMonsters;

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
            m_myMonsters = new List<GameObject>();

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


    // ==================== PUNRPC ====================
    public void ShowModel(GameObject model, Vector3 position, Quaternion rotation)
    {
        photonView.RPC("ShowModel_RPC", RpcTarget.All, model, position, rotation);
    }

    [PunRPC]
    private void ShowModel_RPC(GameObject model, Vector3 position, Quaternion rotation)
    {
        if (!model.activeSelf)
        {
            model.SetActive(true);
        }
        model.transform.position = position;
        model.transform.rotation = rotation;
    }



    public void EndMyTurn()
    {
        //PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "EndMyTurn_RPC");
        //photonView.RPC("EndMyTurn_RPC", RpcTarget.AllBuffered);
        //PhotonNetwork.SendAllOutgoingCommands();
        photonView.RPC("EndMyTurn_RPC", RpcTarget.All);
    }

    [PunRPC]
    private void EndMyTurn_RPC()
    {
        IsMyTurn = false;
        if (Opponent != null)
        {
            Opponent.IsMyTurn = true;
        }
    }


    public void FindOpponent()
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
        if (PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "GetFirstHalfFields_RPC");
            //photonView.RPC("GetFirstHalfFields_RPC", RpcTarget.AllBuffered);
            //PhotonNetwork.SendAllOutgoingCommands();
            photonView.RPC("GetFirstHalfFields_RPC", RpcTarget.All);

        }
        else
        {
            //PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "GetSecondHalfFields_RPC");
            //photonView.RPC("GetSecondHalfFields_RPC", RpcTarget.AllBuffered);
            //PhotonNetwork.SendAllOutgoingCommands();
            photonView.RPC("GetSecondHalfFields_RPC", RpcTarget.All);
        }
    }
    [PunRPC]
    private void GetFirstHalfFields_RPC()
    {
        for (int i = 0; i < 5; ++i)
        {
            MyPlayFields.Add(GameManager.Instance.playFields[i]);
        }
    }
    [PunRPC]
    private void GetSecondHalfFields_RPC()
    {
        for (int i = 0; i < 5; ++i)
        {
            MyPlayFields.Add(GameManager.Instance.playFields[i + 5]);
        }
    }


}

    



public enum PlayerStateEnum
{
    WAIT,
    CHOOSE_CARD,
    DISPLAY_MODEL,
    ATTACK
}