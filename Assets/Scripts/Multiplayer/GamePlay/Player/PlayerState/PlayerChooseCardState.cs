using UnityEngine;

public class PlayerChooseCardState : MyStateMachine
{
    public Player m_player;

    public PlayerStateEnum name = PlayerStateEnum.CHOOSE_CARD;

    public PlayerChooseCardState(Player player)
    {
        m_player = player;
    }
    protected override void DoBehavior()
    {
        Debug.Log("CHOOSE CARD");
        m_player.CurrentStateName = name;

        if (!m_player.IsMyTurn || m_player.OnAttack || m_player.MonsterChosenID != -1)
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
        else if (m_player.OnAttack)
        {
            m_player.SwitchState(PlayerStateEnum.ATTACK);
        }
        else if(m_player.MonsterChosenID != -1)
        {
            m_player.SwitchState(PlayerStateEnum.DISPLAY_MODEL);
        }
    }

    protected override void Initialize()
    {
        m_player.ChoseNewMonster(-1);
        StateInitialized = true;
    }
}
