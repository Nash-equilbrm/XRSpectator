using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoPanel : MonoBehaviour
{
    public CardConfig m_config;
    public TMPro.TMP_Text m_text;
    public Image m_image;


    private void Update()
    {
        if(GameManager.Instance.playerManager != null)
        {
            transform.LookAt(GameManager.Instance.playerManager.transform);
        }
    }

    public void InitPanel(CardConfig config)
    {
        m_image.sprite = config.avatarImg;
        m_text.text = config.monsterName + "\n"
            + "HP: " + config.HP + "\n"
            + "ATK: " + config.ATK; 
    }
}
