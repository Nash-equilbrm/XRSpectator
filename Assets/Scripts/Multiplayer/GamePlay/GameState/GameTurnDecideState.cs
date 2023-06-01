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
            StateInitialized = true;
        }

    }
    protected override void DoBehavior()
    {
        Debug.Log("Game turn decide state DoBehavior");
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.IsMyTurn = true;
        }
        else
        {
            GameManager.Instance.IsMyTurn = false;
        }
        ExitState = true;
    }
    protected override void Exit()
    {
        Debug.Log("Game turn decide state Exit");
        GameManager.Instance.SwitchState(GameStateEnum.PLAY_GAME); 
    }
    
    
}
