using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTurnDecideState : GameState
{

    protected override void Initialize()
    {
        Debug.Log("Game turn decide state Init");
        if (PhotonNetwork.IsMasterClient)
        {
            if (!GameManager.Instance.startGameBtn.activeSelf)
            {
                GameManager.Instance.startGameBtn.SetActive(true);
            }

            // if player press start button
            if (GameManager.Instance.StartGame)
            {
                GameManager.Instance.startGameBtn.SetActive(false);
                StateInitialized = true;
            }
        }

       

    }
    protected override void DoBehavior()
    {
        Debug.Log("Game turn decide state DoBehavior");
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.playerManager.IsMyTurn = true;
        }
        else
        {
            GameManager.Instance.playerManager.IsMyTurn = false;
        }
        ExitState = true;
    }
    protected override void Exit()
    {
        Debug.Log("Game turn decide state Exit");
        GameManager.Instance.SwitchState(GameStateEnum.PLAY_GAME); 
    }
    
    
}
