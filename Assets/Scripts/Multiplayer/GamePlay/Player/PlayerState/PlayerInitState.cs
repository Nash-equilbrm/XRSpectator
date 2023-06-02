using Photon.Pun;
using System;
using UnityEngine;

public class PlayerInitState : MyStateMachine
{
    public Player m_player;
    public PlayerInitState(Player player)
    {
        m_player = player;
    }

    protected override void DoBehavior()
    {
        Debug.Log("INIT");
        if (GameManager.Instance.PlayerReady)
        {
            m_player.GetPlayFields();
            InitPlayUIs();
            m_player.FindOpponent();
            ExitState = true;
        }
    }


    protected override void Exit()
    {
        m_player.SwitchState(PlayerStateEnum.WAIT);
    }

    protected override void Initialize()
    {
        StateInitialized = true;
    }


   

    private void InitPlayUIs()
    {
        for (int i = 0; i < GameManager.Instance.cardMenuSlots.Length; ++i)
        {
            GameObject cardObj = GameObject.Instantiate(GameManager.Instance.cardPrefab);
            cardObj.transform.SetParent(GameManager.Instance.cardMenuSlots[i]);
            cardObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            CardDisplay card = cardObj.GetComponent<CardDisplay>();
            card.Config = GameManager.Instance.cardConfigs[m_player.cardCollectionIds[i]];
            card.InitCardUI();

        }
    }

}
