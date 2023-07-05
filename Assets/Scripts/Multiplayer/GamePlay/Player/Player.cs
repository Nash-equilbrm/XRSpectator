using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.CameraSystem;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviourPunCallbacks
{
    public PlayerStateEnum CurrentStateName = PlayerStateEnum.NONE;
    public CardConfig[] cardConfigs;

    public int PlayerID { get => m_playerID; }
    [SerializeField] private int m_playerID = -1;


    [SerializeField] private GameObject m_invalidSign = null;
    public ParticleSystem DeathEffect { get => m_deathEffect; }
    [SerializeField] private ParticleSystem m_deathEffect = null;

    public PhotonView m_photonView;
    public CardFieldsMovement cardDisplayMovementControll;


    private MyStateMachine m_currentState;
    //private PlayerChooseCardState m_chooseCardState;
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

    public Monster CurrentMonster { get => m_currentMonster; }
    [SerializeField] private Monster m_currentMonster;



    // gameobject instances => for pointing gesture when displaying model state
    public List<GameObject> MyCardDisplayFields { get => m_myCardDisplayFields; }
    [SerializeField] private List<GameObject> m_myCardDisplayFields;
    // class instances => basically the same
    public CardDisplayField[] cardFields;
    public PlayField m_playField;

    public Dictionary<int, Monster> MyMonsters { get => m_myMonsters; }
    [SerializeField] private Dictionary<int, Monster> m_myMonsters;

    public List<int> DebugMonsterList = new List<int>();


    public int HP { get => m_HP; }

    private int m_HP = 1000;


    void Start()
    {
        m_photonView = GetComponent<PhotonView>();
        m_myMonsters = new Dictionary<int, Monster>();

        if (m_photonView.IsMine)
        {
            //m_chooseCardState = new PlayerChooseCardState(this);
            m_displayModelState = new PlayerDisplayModelState(this);
            m_waitState = new PlayerWaitState(this);
            m_attackState = new PlayerAttackState(this);
            m_initState = new PlayerInitState(this);

            m_currentState = m_initState;

            GameObject sign = PhotonNetwork.Instantiate("Prefabs/Menus/" + GameManager.Instance.invalidSignPrefab.name, new Vector3(100, 100, 100), Quaternion.identity);
            GetInvalidSign(sign.GetComponent<PhotonView>().ViewID);

            GameObject deathEffect = PhotonNetwork.Instantiate("Prefabs/Effects/" + GameManager.Instance.deathEffectPrefab.name, new Vector3(100, 100, 100), Quaternion.identity);
            GetDeathEffect(deathEffect.GetComponent<PhotonView>().ViewID);
        }

    }

    void Update()
    {
        if (m_photonView.IsMine && GameManager.Instance.TrackedWithVuforia && GameManager.Instance.GameResult == GameResultEnum.NONE)
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


    // ==================== PUNRPC ====================

    public void SwitchState(PlayerStateEnum nextState)
    {
        PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "SwitchState_RPC");
        m_photonView.RPC("SwitchState_RPC", RpcTarget.AllBuffered, nextState);
        PhotonNetwork.SendAllOutgoingCommands();
    }


    [PunRPC]
    private void SwitchState_RPC(PlayerStateEnum nextState)
    {
        switch (nextState)
        {
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


   


    public void SetReady(bool ready)
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "SetReady_RPC");
            m_photonView.RPC("SetReady_RPC", RpcTarget.AllBuffered, ready);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void SetReady_RPC(bool ready)
    {
        Debug.Log(m_playerID + " SetReady: " + ready);
        m_isReady = ready;
    }


    public void StartMyTurn(bool startTurn)
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "StartMyTurn_RPC");
            m_photonView.RPC("StartMyTurn_RPC", RpcTarget.AllBuffered, startTurn);
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
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "DoAttack_RPC");
            m_photonView.RPC("DoAttack_RPC", RpcTarget.AllBuffered, attack);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    public void DoAttack_RPC(bool attack)
    {
        m_onAttack = attack;
    }



    public void EndMyTurn()
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "EndMyTurn_RPC");
            m_photonView.RPC("EndMyTurn_RPC", RpcTarget.AllBuffered);
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
        PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "FindOpponent_RPC");
        m_photonView.RPC("FindOpponent_RPC", RpcTarget.AllBuffered);
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


    public void AddNewMonster(int monsterViewID)
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "AddNewMonster_RPC");
            m_photonView.RPC("AddNewMonster_RPC", RpcTarget.AllBuffered, monsterViewID);
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
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "RemoveMonster_RPC");
            m_photonView.RPC("RemoveMonster_RPC", RpcTarget.AllBuffered, monsterViewID);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    private void RemoveMonster_RPC(int monsterViewID)
    {
        if (m_myMonsters.ContainsKey(monsterViewID))
        {
            if (m_currentMonster.m_photonView.ViewID == monsterViewID) m_currentMonster = null;
            m_myMonsters.Remove(monsterViewID);
            for(int i = 0; i < DebugMonsterList.Count; ++i)
            {
                if(DebugMonsterList[i] == monsterViewID)
                {
                    DebugMonsterList.RemoveAt(i);
                    return;
                }
            }
        }
    }


    public void SetPlayerID(int ID)
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "SetPlayerID_RPC");
            m_photonView.RPC("SetPlayerID_RPC", RpcTarget.AllBuffered, ID);
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

        PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "GetInvalidSign_RPC");
        m_photonView.RPC("GetInvalidSign_RPC", RpcTarget.AllBuffered, viewID);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    private void GetInvalidSign_RPC(int viewID)
    {
        m_invalidSign = PhotonView.Find(viewID).gameObject;
    }


    public void ShowInvalidSign(Vector3 position, Quaternion rotation, bool active = true)
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "ShowInvalidSign_RPC");
            m_photonView.RPC("ShowInvalidSign_RPC", RpcTarget.AllBuffered, position, rotation, active);
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

        PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "GetDeathEffect_RPC");
        m_photonView.RPC("GetDeathEffect_RPC", RpcTarget.AllBuffered, viewID);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    private void GetDeathEffect_RPC(int viewID)
    {
        GameObject effectObj = PhotonView.Find(viewID).gameObject;
        if (effectObj)
        {
            m_deathEffect = effectObj.GetComponent<ParticleSystem>();
            m_deathEffect.gameObject.SetActive(false);
        }
    }

    public void PlayDeathEffect(Vector3 position)
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "PlayDeathEffect_RPC");
            m_photonView.RPC("PlayDeathEffect_RPC", RpcTarget.AllBuffered, position);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    private void PlayDeathEffect_RPC(Vector3 position)
    {
        DeathEffect.transform.position = position;
        DeathEffect.gameObject.SetActive(true);
        DeathEffect.Play();
    }


    public void ResetTurn()
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "ResetTurn_RPC");
            m_photonView.RPC("ResetTurn_RPC", RpcTarget.AllBuffered);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    private void ResetTurn_RPC()
    {
        foreach (int monsterID in m_myMonsters.Keys)
        {
            m_myMonsters[monsterID].ResetMonsterAttack();
        }
    }

    public void UpdateNewMonster()
    {
        if (m_photonView.IsMine)
        {
            PhotonNetwork.RemoveBufferedRPCs(m_photonView.ViewID, "UpdateNewMonster_RPC");
            m_photonView.RPC("UpdateNewMonster_RPC", RpcTarget.AllBuffered);
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    [PunRPC]
    private void UpdateNewMonster_RPC()
    {
        m_currentMonster = m_playField.CurrentCardDisplay.Monster;
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