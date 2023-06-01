using UnityEngine;

public class PlayerChooseCardState : MyStateMachine
{
    public Player m_player;
    public PlayerChooseCardState(Player player)
    {
        m_player = player;
    }
    protected override void DoBehavior()
    {
        Debug.Log("CHOOSE CARD");

        if (!m_player.IsMyTurn || m_player.OnAttackPhase || m_player.CardChose != null)
        {
            ExitState = true;
        }
    }

    protected override void Exit()
    {
        if (!m_player.IsMyTurn)
        {
            m_player.EndMyTurn();
            m_player.SwitchState(PlayerStateEnum.WAIT);
        }
        else if (m_player.OnAttackPhase)
        {
            m_player.SwitchState(PlayerStateEnum.ATTACK);
        }
        else if(m_player.CardChose != null)
        {
            m_player.SwitchState(PlayerStateEnum.DISPLAY_MODEL);
        }
    }

    protected override void Initialize()
    {
        m_player.CardChose = null;
        StateInitialized = true;
    }
}
