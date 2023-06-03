using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTurnDecideState : MyStateMachine
{
    protected override void Initialize()
    {
        Debug.Log("Game turn decide state Init");
        // if player press start button
        if (GameManager.Instance.PlayerReady)
        {
            InitPlayer();  
            StateInitialized = true;
        }

    }
    protected override void DoBehavior()
    {
        Debug.Log("Game turn decide state DoBehavior");
        if (PhotonNetwork.IsMasterClient)
        {
            InitPlayerTurn(true);
        }
        else
        {
            InitPlayerTurn(false);
        }
        ExitState = true;
    }
    protected override void Exit()
    {
        Debug.Log("Game turn decide state Exit");
        GameManager.Instance.SwitchState(GameStateEnum.PLAY_GAME); 
    }
    
    private void InitPlayer()
    {
        GameManager gameManager = GameManager.Instance;

        GameObject playerModel = PhotonNetwork.Instantiate("Prefabs/" + gameManager.playerAvatarPrefab.name, gameManager.ARCamera.transform.position, gameManager.ARCamera.transform.rotation);
        if (playerModel.GetComponent<PhotonView>().IsMine)
        {
            playerModel.GetComponent<MoveARCamera>().ARCamera = gameManager.ARCamera.transform;
            gameManager.playerManager = playerModel.GetComponent<Player>();
            if (PhotonNetwork.IsMasterClient)
            {
                gameManager.playerManager.SetPlayerID(1);
            }
            else
            {
                gameManager.playerManager.SetPlayerID(2);
            }
        }
    }


    private void InitPlayerTurn(bool playerGoFirst)
    {
        if(GameManager.Instance.playerManager != null)
        {
            GameManager.Instance.playerManager.StartMyTurn(playerGoFirst);
        }
    }
    
}
