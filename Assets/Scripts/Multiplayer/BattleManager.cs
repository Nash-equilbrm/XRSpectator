using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{
    [Header("Gameplay")]
    public GameObject turnDecideCoinPrefab;
    public GameObject invalidSignPrefab;
    public GameObject[] playFields;
    public GameObject cardPrefab;
    public Transform[] cardMenuSlots;
    public CardConfig[] cardConfigs;

    public GameResultEnum GameResult { get => gameResult; set => gameResult = value; }

    private GameResultEnum gameResult = GameResultEnum.NONE;

    // game states
    private MyStateMachine currentState = null;
    private GameTurnDecideState turnDecideState = new GameTurnDecideState();
    private GamePlayState playState = new GamePlayState();

    public bool PlayerReady { get => playerReady; set => playerReady = value; }
    [SerializeField] private bool playerReady = false;

    public bool IsMyTurn { get => isMyTurn; set => isMyTurn = value; }
    [SerializeField] private bool isMyTurn = false;

    public bool OnAttack { get => onAttack; set => onAttack = value; }
    [SerializeField] private bool onAttack = false;


    private void UpdateGameplay()
    {
        InitGameStates();
        currentState.UpdateState();
    }

    private void InitGameStates()
    {
        if (currentState == null)
        {
            currentState = turnDecideState;
        }
    }


    public void SwitchState(GameStateEnum nextGameState)
    {
        switch (nextGameState)
        {
            case GameStateEnum.TURN_DECIDE:
                {
                    currentState = turnDecideState;
                    break;
                }
            case GameStateEnum.PLAY_GAME:
                {
                    currentState = playState;
                    break;
                }
        }
    }
   
}

public enum GameStateEnum
{
    TURN_DECIDE,
    PLAY_GAME,
    END_GAME
}

public enum GameResultEnum
{
    WIN,
    LOSE,
    NONE
}
