using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayState : MyStateMachine
{
    protected override void Initialize()
    {
        Debug.Log("Game play state Init");
        StateInitialized = true;
    }

    protected override void DoBehavior()
    {
        Debug.Log("Game play state DoBehavior");

        if(GameManager.Instance.playerManager.MyMonsters.Count <= 0 && GameManager.Instance.playerManager.Opponent.MyMonsters.Count > 0)
        {
            GameManager.Instance.GameResult = GameResultEnum.LOSE;
            ExitState = true;
        }
        else if(GameManager.Instance.playerManager.MyMonsters.Count > 0 && GameManager.Instance.playerManager.Opponent.MyMonsters.Count <= 0)
        {
            GameManager.Instance.GameResult = GameResultEnum.WIN;
            ExitState = true;
        }
        else if(GameManager.Instance.playerManager.MyMonsters.Count == GameManager.Instance.playerManager.Opponent.MyMonsters.Count && GameManager.Instance.playerManager.MyMonsters.Count == 0)
        {
            GameManager.Instance.GameResult = GameResultEnum.DRAW;
            ExitState = true;
        }

    }

    protected override void Exit()
    {
        Debug.Log("Game play state Exit");
        // show result panels
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject gameResultPanel = PhotonNetwork.Instantiate("Menus/" + GameManager.Instance.gameResultPrefab.name, new Vector3(0, 1.5f, 0), Quaternion.identity);
            Vector3 rotation = gameResultPanel.transform.eulerAngles;

            if (GameManager.Instance.playerManager.PlayerID == 1)
            {
                if(GameManager.Instance.GameResult == GameResultEnum.WIN)
                {
                    rotation.y = 0;
                }
                else if (GameManager.Instance.GameResult == GameResultEnum.WIN)
                {
                    rotation.y = 180;
                }

                gameResultPanel.transform.eulerAngles = rotation;
            }
            else
            {
                if (GameManager.Instance.GameResult == GameResultEnum.WIN)
                {
                    rotation.y = 180;
                }
                else if (GameManager.Instance.GameResult == GameResultEnum.WIN)
                {
                    rotation.y = 0;
                }

                gameResultPanel.transform.eulerAngles = rotation;
            }
        }
    }


}
