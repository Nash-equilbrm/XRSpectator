using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;
using System;
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
    private PlayerInitState m_initState;


    public Player Opponent { get => m_opponent; set => m_opponent = value; }
    private Player m_opponent;

    public bool IsMyTurn { get => m_isMyTurn; set => m_isMyTurn = value; }
    [SerializeField] private bool m_isMyTurn = false;
    

    public Card CardChose { get => m_cardChose; set => m_cardChose = value; }
    private Card m_cardChose = null;


    public bool PlayerReady { get => playerReady; set => playerReady = value; }
    private bool playerReady = false;


    void Start()
    {
        if (GameManager.Instance.isAudience)
        {
            Destroy(gameObject);
        }
        else
        {
            photonView = GetComponent<PhotonView>();

            m_chooseCardState = new PlayerChooseCardState(this);
            m_displayModelState = new PlayerDisplayModelState(this);
            m_waitState = new PlayerWaitState(this);
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

    public void EndMyTurn()
    {
        PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "NoticeOpponentTurn");
        photonView.RPC("NoticeOpponentTurn", RpcTarget.AllBuffered);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    private void NoticeOpponentTurn()
    {
        IsMyTurn = false;
        if(Opponent != null)
        {
            Opponent.IsMyTurn = true;
        }
    }


    public void SwitchState(PlayerStateEnum nextState)
    {
        switch (nextState)
        {
            case PlayerStateEnum.CHOOSE_CARD:
                {
                    Debug.Log("switch to CHOOSE_CARD");
                    m_currentState = m_chooseCardState; 
                    break;
                }
            case PlayerStateEnum.DISPLAY_MODEL:
                {
                    Debug.Log("switch to DISPLAY_MODEL");
                    m_currentState = m_displayModelState;
                    break;
                }
            case PlayerStateEnum.WAIT:
                {
                    Debug.Log("switch to WAIT");
                    m_currentState = m_waitState;
                    break;
                }
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