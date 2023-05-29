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
        if (m_player.PlayerReady)
        {
            InitPlayUIs();
            FindOpponent();
            ExitState = true;
        }
    }

    protected override void Exit()
    {
        m_player.SwitchState(PlayerStateEnum.WAIT);
    }

    protected override void Initialize()
    {
        StateInitialized = true;
    }


    private void FindOpponent()
    {
        if (m_player.Opponent == null)
        {
            m_player.gameObject.tag = "Temp";
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj)
            {
                m_player.Opponent = obj.GetComponent<Player>();
            }
            m_player.gameObject.tag = "Player";
            Debug.Log("Found opponent");

        }
        //if (m_player.Opponent == null)
        //{
        //    GameObject obj = PhotonNetwork.Instantiate("Prefabs/" + GameManager.Instance.playerAvatarPrefab.name, new Vector3(0, 5, 0), Quaternion.identity);
        //    m_player.Opponent = obj.GetComponent<Player>();
        //    Debug.Log("Cannot find opponent: create one");
        //}
    }

    private void InitCardUIs()
    {
        Debug.Log("InitCardUIs");
        for (int i = 0; i < m_player.cardConfigs.Length; ++i)
        {
            GameObject cardObj = GameObject.Instantiate(m_player.cardPrefab);
            cardObj.transform.SetParent(m_player.cardMenuSlots[i]);
            cardObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            Card card = cardObj.GetComponent<Card>();
            card.Config = m_player.cardConfigs[i];
            card.InitCardUI();
        }
    }

    private void InitPlayUIs()
    {
        m_player.startGameBtn.SetActive(false);
        InitCardUIs();

    }

}
