using UnityEngine;

public class PlayerWaitState : MyStateMachine
{

    public Player m_player;
    public PlayerWaitState(Player player)
    {
        m_player = player;
    }

    protected override void DoBehavior()
    {
        Debug.Log("WAIT");
        if (GameManager.Instance.playerManager.OpponentEndTurn || GameManager.Instance.IsMyTurn)
        {
            ExitState = true;
        }
        else
        {
            m_player.ChooseNewCardInDeck(-1);
        }
    }

    protected override void Exit()
    {
        if (GameManager.Instance.playerManager.OpponentEndTurn)
        {
            GameManager.Instance.IsMyTurn = true;
        }
        m_player.SwitchState(PlayerStateEnum.CHOOSE_CARD);
    }

    protected override void Initialize()
    {
        StateInitialized = true;
    }
}
