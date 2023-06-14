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
        m_player.FindOpponent();

        if (m_player.Opponent != null)
        {
        }
            ExitState = true;
    }


    protected override void Exit()
    {
        m_player.SwitchState(PlayerStateEnum.WAIT);
    }

    protected override void Initialize()
    {
        if (GameManager.Instance.PlayerReady)
        {
            if (m_player.photonView.IsMine)
            {
                m_player.GetPlayFields();
                InitPlayUIs();
                StateInitialized = true;
            }
        }
    }


   

    private void InitPlayUIs()
    {
        for (int i = 0; i < GameManager.Instance.cardMenuSlots.Length; ++i)
        {
            GameObject cardObj = GameManager.Instance.cardMenuSlots[i];
            cardObj.SetActive(true);
            Card card = cardObj.GetComponent<Card>();
            card.InitCardUI(i, GameManager.Instance.cardConfigs[m_player.m_cardCollectionIds[0]]);

            m_player.m_cardCollectionIds.RemoveAt(0);
        }
    }

}
