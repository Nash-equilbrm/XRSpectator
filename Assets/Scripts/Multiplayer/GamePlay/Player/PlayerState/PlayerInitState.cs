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
        if (m_player.PlayerReady)
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
        m_player.startGameBtn.SetActive(false);
        for (int i = 0; i < m_player.cardConfigs.Length; ++i)
        {
            GameObject cardObj = GameObject.Instantiate(m_player.cardPrefab);
            cardObj.transform.SetParent(m_player.cardMenuSlots[i]);
            cardObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            Card card = cardObj.GetComponent<Card>();
            card.Config = m_player.cardConfigs[i];
            card.InitCardUI();
        }
    }

}
