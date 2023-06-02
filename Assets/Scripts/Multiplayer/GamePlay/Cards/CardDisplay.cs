using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public CardConfig Config { get => m_config; set => m_config = value; }
    private CardConfig m_config;

    public Monster Monster { get => m_monster; set => m_monster = value; }
    private Monster m_monster = null;

    public Image avatarGUIImg;


    public void InitCardUI()
    {
        if(Config != null)
        {
            if(Config.avatarImg != null)
                avatarGUIImg.sprite = Config.avatarImg;
            if(Config.model != null)
            {
                GameObject monster = PhotonNetwork.Instantiate("Prefabs/Monsters/" + Config.model.name, new Vector3(100,100,100), Quaternion.identity);
                Monster = monster.GetComponent<Monster>();
                if (Monster != null)
                {
                    Monster.SetUpStats(Config);
                    Monster.SetMonsterTag("Creature" + GameManager.Instance.playerManager.PlayerID.ToString());
                }
            }

            GameManager.Instance.playerManager.AddNewMonster(Monster.photonView.ViewID);

        } 
    }


    public void OnCardPressed()
    {
        Debug.Log("OnCardPressed");
        GameManager.Instance.playerManager.ChoseNewMonster(Monster.photonView.ViewID);
    }
   
}
