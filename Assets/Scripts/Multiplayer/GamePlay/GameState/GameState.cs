using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
    public bool StateInitialized { get => m_stateInitialized; set => m_stateInitialized = value; }
    private bool m_stateInitialized;

    public bool ExitState { get => m_exitState; set => m_exitState = value; }
    private bool m_exitState;

    protected abstract void Initialize();
    protected abstract void DoBehavior();
    protected abstract void Exit();
    public void UpdateState()
    {
        if (!StateInitialized)
        {
            Initialize();
        }
        else if (!ExitState)
        {
            DoBehavior();
        }
        else
        {
            Exit();
        }
    }
}
