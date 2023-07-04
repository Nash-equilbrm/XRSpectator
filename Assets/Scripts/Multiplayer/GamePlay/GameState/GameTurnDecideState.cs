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

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if(playerObj == null)
            {
                GameManager.Instance.CreatePlayerAvatar(0);
                GameManager.Instance.playerManager?.SetReady(true);
            }
            else
            {
                GameManager.Instance.CreatePlayerAvatar(1);
                GameManager.Instance.playerManager?.SetReady(true);
            }


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
    

    private void InitPlayerTurn(bool playerGoFirst)
    {
        if(GameManager.Instance.playerManager != null)
        {
            GameManager.Instance.playerManager.StartMyTurn(playerGoFirst);
        }
    }
    
}
