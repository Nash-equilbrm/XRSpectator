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
            ExitState = true;
        }
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
        int slotCnt = GameManager.Instance.cardMenuSlots.Length;
        int cardCnt = GameManager.Instance.playerManager.CardCollection.Count;
        int cnt = (slotCnt < cardCnt) ? slotCnt : cardCnt;
        for (int i = 0; i < cnt; ++i)
        {
            GameObject cardObj = GameManager.Instance.cardMenuSlots[i];
            cardObj.SetActive(true);
            Card card = cardObj.GetComponent<Card>();
            card.InitCardUI(i, GameManager.Instance.cardConfigs[m_player.CardCollection[0]]);

            m_player.RemoveRemainCard(0);
        }
    }

}
