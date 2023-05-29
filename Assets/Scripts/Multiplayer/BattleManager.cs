using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{
    [Header("Gameplay")]
    public GameObject turnDecideCoinPrefab;
    public Transform[] cardMenuSlots;
    public GameObject cardPrefab;
    public GameObject startGameBtn;

    public CardConfig[] cardConfigs;

   
    public bool PlayerReady { get => playerUIInitialized; set => playerUIInitialized = value; }
    private bool playerUIInitialized = false;



    public bool StartGame { get => startGame; set => startGame = value; }
    private bool startGame = false;


    public GameResultEnum GameResult { get => gameResult; set => gameResult = value; }
    private GameResultEnum gameResult = GameResultEnum.NONE;

    // game states
    private GameState currentState = null;
    private GameTurnDecideState turnDecideState = new GameTurnDecideState();
    private GamePlayState playState = new GamePlayState();



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

    public void InitCardUIs()
    {
        if (TrackedWithVuforia && !PlayerReady)
        {
            Debug.Log("InitUI");
            for (int i = 0; i < cardConfigs.Length; ++i)
            {
                GameObject cardObj = Instantiate(cardPrefab);
                cardObj.transform.SetParent(cardMenuSlots[i]);
                cardObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                Card card = cardObj.GetComponent<Card>();
                card.Config = cardConfigs[i];
                card.InitCardUI();
            }
            PlayerReady = true;
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
