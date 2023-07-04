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
        m_player.FindOpponent();

        if (m_player.Opponent != null)
        {
            if(m_player.PlayerID == -1)
            {
                // if find another player and PlayerID not set => set ID = 1 (Second Player)
                if (!m_player.Opponent.IsReady)
                {
                    m_player.SetPlayerID(0);
                }
                else
                {
                    m_player.SetPlayerID(1);
                }
            }
            ExitState = true;
        }
        // if not find player and PlayerID is not set => set ID = 0 (First player)
        else
        {
            m_player.SetPlayerID(0);
        }

    }


    protected override void Exit()
    {
        if (m_player.Opponent.IsReady)
        {
            m_player.SwitchState(PlayerStateEnum.WAIT);
        }
    }

    protected override void Initialize()
    {
        if (GameManager.Instance.PlayerReady)
        {
            if (m_player.m_photonView.IsMine)
            {
                InitCardAndMonster();
                StateInitialized = true;
            }
        }
    }




    private void InitCardAndMonster()
    {
        for (int i = 0; i < m_player.CardCollection.Count; ++i)
        {
            Debug.Log("InitPlayUIs: " + i);
            // create gameobject of monster
            CardConfig config = m_player.cardConfigs[m_player.CardCollection[i]];
            GameObject monsterObj = PhotonNetwork.Instantiate("Prefabs/Monsters/" + config.model.name, new Vector3(100, 100, 100), Quaternion.identity);

            // set up display card field for that monster
            m_player.cardFields[i].SetObjectActive(true);
            m_player.cardFields[i].ChangeImage(config.configID);
            m_player.cardFields[i].SetNewMonster(monsterObj.GetPhotonView().ViewID);


            // put monster to manager list
            Monster monster = monsterObj.GetComponent<Monster>();
            monster.SetUpStats(config);
            if (monster.m_photonView.IsMine)
            {
                m_player.AddNewMonster(monster.m_photonView.ViewID);
            }
            monster.SetMonsterReady(false);


        }

    }

}
