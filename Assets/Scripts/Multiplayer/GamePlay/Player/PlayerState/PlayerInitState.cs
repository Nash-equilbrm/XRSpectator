using Photon.Pun;
using System;
using System.Collections.Generic;
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
            if (m_player.PlayerID == 0)
            {
                List<int> randomCardIDs = GetRandomCards();
                m_player.GetCardCollection(randomCardIDs.GetRange(0, m_player.cardConfigs.Length / 2).ToArray());
                m_player.Opponent.GetCardCollection(randomCardIDs.GetRange(m_player.cardConfigs.Length / 2, m_player.cardConfigs.Length / 2).ToArray());
            }

            if(m_player.CardCollection.Count > 0 && m_player.Opponent.CardCollection.Count > 0)
            {
                InitCardAndMonster();
                ExitState = true;
            }

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


    private List<int> GetRandomCards()
    {
        List<int> list = new List<int>();
        for(int i = 0; i < m_player.cardConfigs.Length; ++i)
        {
            list.Add(i);
        }

        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

}
