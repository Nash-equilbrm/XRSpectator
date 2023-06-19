using UnityEngine;

public class PlayerWaitState : MyStateMachine
{

    public Player m_player;

    public PlayerStateEnum name = PlayerStateEnum.WAIT;

    public PlayerWaitState(Player player)
    {
        m_player = player;
    }

    protected override void DoBehavior()
    {
        Debug.Log("WAIT");
        m_player.CurrentStateName = name;

        if (m_player.IsMyTurn)
        {
            m_player.StartMyTurn(true);
            ExitState = true;
            return;
        }
    }

    protected override void Exit()
    {
        m_player.SwitchState(PlayerStateEnum.CHOOSE_CARD);
    }

    protected override void Initialize()
    {
        m_player.ChoseNewMonster(-1);
        // reset attack so that monsters are able to attack in the new turn.
        m_player.ResetTurn();
        StateInitialized = true;
    }
}
