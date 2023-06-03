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
        if (m_player.IsMyTurn)
        {
            m_player.StartMyTurn(true);
            ExitState = true;
        }
        else
        {
            m_player.ChoseNewMonster(-1);
        }
    }

    protected override void Exit()
    {
        m_player.SwitchState(PlayerStateEnum.CHOOSE_CARD);
    }

    protected override void Initialize()
    {
        StateInitialized = true;
    }
}
