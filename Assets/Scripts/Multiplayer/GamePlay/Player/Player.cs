using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;
using System;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    private enum PlayerState
    {
        CHOOSE,
        DISPLAY_MODEL,
        WAIT
    }
    private PlayerState currentPlayerState;

    public Player opponent;
    public PhotonView photonView;
    public bool IsMyTurn { get => isMyTurn; set => isMyTurn = value; }
    [SerializeField] private bool isMyTurn = false;
    

    public Card CardChose { get => cardChose; set => cardChose = value; }
    private Card cardChose = null;


    void Start()
    {
        if (GameManager.Instance.isAudience)
        {
            Destroy(gameObject);
        }
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (GameManager.Instance.TrackedWithVuforia && GameManager.Instance.PlayerReady)
        {
            FindOpponent();
            UpdatePlayer();
        }
    }

    private void FindOpponent()
    {
        if(opponent == null)
        {
            gameObject.tag = "Temp";
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj)
            {
                opponent = obj.GetComponent<Player>();
            }
            gameObject.tag = "Player";
            Debug.Log("Found opponent");

        }
        if (opponent == null)
        {
            GameObject obj = PhotonNetwork.Instantiate("Prefabs/" + GameManager.Instance.playerAvatarPrefab.name, new Vector3(0,5,0), Quaternion.identity);
            opponent = obj.GetComponent<Player>();
            Debug.Log("Cannot find opponent: create one");

        }
    }

    private float chooseModelPositionDuration = 2f;
    private Vector3 modelPrevPos = Vector3.zero;
    private void UpdatePlayer()
    {
        switch (currentPlayerState)
        {
            case PlayerState.CHOOSE:
                {
                    Debug.Log("CHOOSE CARD");
                    if (CardChose != null)
                    {
                        currentPlayerState = PlayerState.DISPLAY_MODEL;
                    }
                    break;
                }
            case PlayerState.DISPLAY_MODEL:
                {
                    Debug.Log("DISPLAY_MODEL");
                    var raycastHit = GetRayCastHit();
                    if (raycastHit != null && raycastHit.Details.Object.tag == "PlayField")
                    {
                        ShowModel(raycastHit.Details.Object.transform.position, Quaternion.identity);
                    }
                    if (modelPrevPos != Vector3.zero && Vector3.Distance(CardChose.Model.transform.position, modelPrevPos) <= 0.3f)
                    {
                        if (chooseModelPositionDuration <= 0f)
                        {
                            chooseModelPositionDuration = 2f;
                            modelPrevPos = Vector3.zero;
                            CardChose = null;

                            // End my turn

                            PhotonNetwork.RemoveBufferedRPCs(photonView.ViewID, "NoticeOpponentTurn");
                            photonView.RPC("NoticeOpponentTurn", RpcTarget.AllBuffered);
                            PhotonNetwork.SendAllOutgoingCommands();

                            currentPlayerState = PlayerState.WAIT;
                            
                            break;
                        }
                        else
                        {
                            chooseModelPositionDuration -= Time.deltaTime;
                        }
                    }
                    else
                    {
                        chooseModelPositionDuration = 2f;
                    }
                    modelPrevPos = CardChose.Model.transform.position;

                    break;

                };
            case PlayerState.WAIT:
                {
                    Debug.Log("WAIT");
                    if (IsMyTurn)
                    {
                        currentPlayerState = PlayerState.CHOOSE;
                    }
                    else
                    {
                        CardChose = null;
                    }
                    break;
                }
        }
    }

    private IPointerResult GetRayCastHit()
    {
        foreach (var source in MixedRealityToolkit.InputSystem.DetectedInputSources)
        {
            // Ignore anything that is not a hand because we want articulated hands
            if (source.SourceType == InputSourceType.Hand)
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
                            return p.Result;
                        }
                    }

                }
            }
        }
        return null;
    }

    

    public void ShowModel(Vector3 position, Quaternion rotation)
    {
        if (!cardChose.Model.activeSelf)
        {
            cardChose.Model.SetActive(true);
        }
        cardChose.Model.transform.position = position;
        cardChose.Model.transform.rotation = Quaternion.identity;

    }


    [PunRPC]
    private void NoticeOpponentTurn()
    {
        IsMyTurn = false;
        if(opponent != null)
        {
            opponent.IsMyTurn = true;
        }
    }
}
