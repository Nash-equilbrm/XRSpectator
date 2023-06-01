using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayState : MyStateMachine
{
    protected override void Initialize()
    {
        Debug.Log("Game play state Init");
        GameManager.Instance.invalidSign = PhotonNetwork.Instantiate("Prefabs/Menus/" + GameManager.Instance.invalidSignPrefab.name, new Vector3(100, 100, 100), Quaternion.identity);
        GameManager.Instance.invalidSign.SetActive(false);
        StateInitialized = true;
    }

    protected override void DoBehavior()
    {
        Debug.Log("Game play state DoBehavior");
        if (GameManager.Instance.GameResult != GameResultEnum.NONE)
        {
            ExitState = true;
        }
    }

    protected override void Exit()
    {
        Debug.Log("Game play state Exit");
    }


}
