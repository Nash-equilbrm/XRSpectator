using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{
    [Header("Gameplay")]
    public GameObject gameResultPrefab;
    public GameObject turnDecideCoinPrefab;
    public GameObject invalidSignPrefab;
    public GameObject deathEffectPrefab;
    public GameObject guiObj;
    public GameObject startGameBtn;

    public GameResultEnum GameResult { get => gameResult; set => gameResult = value; }

    [SerializeField] private GameResultEnum gameResult = GameResultEnum.NONE;

    // game states
    private MyStateMachine currentState = null;
    private GameTurnDecideState turnDecideState = new GameTurnDecideState();
    private GamePlayState playState = new GamePlayState();
    private GameWaitState endState = new GameWaitState();


    public bool PlayerReady { get => playerReady; }
    [SerializeField] private bool playerReady = false;


    private bool gameplayInit = false;
    private void UpdateGameplay()
    {
        if (TrackedWithVuforia && !gameplayInit)
        {
            guiObj.SetActive(true);
            FollowTransform followTransform = guiObj.GetComponent<FollowTransform>();
            if (followTransform)
            {
                followTransform.follow = playerManager.transform;
            }
            InitGameStates();
            gameplayInit = true;
        }
        currentState.UpdateState();


        // testing
        if (Input.GetKeyDown(KeyCode.T))
        {
            OnStartGamePressed();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            OnAttackPressed();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnEndTurnPressed();
        }
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
            case GameStateEnum.END_GAME:
                {
                    currentState = endState;
                    break;
                }
        }
    }



    public void OnAttackPressed()
    {
        if(playerManager != null)
        {
            playerManager.DoAttack(true);
        }
    }
   

    public void OnEndTurnPressed()
    {
        if(playerManager != null)
        {
            playerManager.ResetTurn();
            playerManager.StartMyTurn(false);
        }
    }

    public void OnStartGamePressed()
    {
        playerReady = true;
        playerManager?.SetReady(true);

        startGameBtn.SetActive(false);
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
    DRAW,
    NONE
}
