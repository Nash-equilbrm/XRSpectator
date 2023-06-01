using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    private int m_index = -1;

    public CardConfig Config { get => m_config; set => m_config = value; }
    private CardConfig m_config;

    public Monster Monster { get => m_monster; set => m_monster = value; }
    private Monster m_monster = null;

    public Image avatarGUIImg;


    public void InitCardUI(int index)
    {
        if(Config != null)
        {
            m_index = index;
            if(Config.avatarImg != null)
                avatarGUIImg.sprite = Config.avatarImg;
            if(Config.model != null)
            {
                GameObject monster = PhotonNetwork.Instantiate("Prefabs/Monsters/" + Config.model.name, new Vector3(100,100,100), Quaternion.identity);
                Monster = monster.GetComponent<Monster>();
                if (Monster != null)
                {
                    Monster.SetUpStats(Config);
                    Monster.SetMonsterTag("Creature" + GameManager.Instance.playerManager.playerID.ToString());
                }
            }

        } 
    }


    public void OnCardPressed()
    {
        Debug.Log("OnCardPressed");
        GameManager.Instance.playerManager.ChooseNewCardInDeck(m_index);
    }
   
}
