using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardConfig Config { get => config; set => config = value; }
    private CardConfig config;
    public GameObject Model { get => model; set => model = value; }
    private GameObject model;

    public Image avatarGUIImg;


    public void InitCardUI()
    {
        if(Config != null)
        {
            if(Config.avatarImg != null)
                avatarGUIImg.sprite = Config.avatarImg;
            if(Config.model != null)
            {
                model = PhotonNetwork.Instantiate("Prefabs/Creatures/" + Config.model.name, new Vector3(100,100,100), Quaternion.identity);
                model.SetActive(false);
            }
        } 
    }


    public void OnCardPressed()
    {
        Debug.Log("OnCardPressed");
        GameManager.Instance.playerManager.CardChose = this;
    }
   
}
