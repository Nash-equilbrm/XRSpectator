using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private int m_index;

    private CardConfig m_config;

    public Monster Monster { get => m_monster; set => m_monster = value; }

    private Monster m_monster = null;

    public Image avatarGUIImg;


    public void InitCardUI(int index, CardConfig config)
    {
        m_index = index;
        m_config = config;

        if (m_config.avatarImg != null)
            avatarGUIImg.sprite = m_config.avatarImg;
        if (m_config.model != null)
        {
            GameObject monster = PhotonNetwork.Instantiate("Prefabs/Monsters/" + m_config.model.name, new Vector3(100, 100, 100), Quaternion.identity);
            Monster = monster.GetComponent<Monster>();
            if (Monster != null)
            {
                Monster.SetUpStats(m_config);
                Monster.SetMonsterTag("Creature" + GameManager.Instance.playerManager.PlayerID.ToString());
            }
        }

        if (Monster.m_photonView.IsMine)
        {
            GameManager.Instance.playerManager.AddNewMonster(Monster.m_photonView.ViewID);
        }
    }


    public void OnCardPressed()
    {
        Debug.Log("OnCardPressed");
        GameManager.Instance.playerManager.ChoseNewMonster(Monster.m_photonView.ViewID);
        GameManager.Instance.playerManager.ChooseNewCard(m_index, m_config.configID);
    }

}
