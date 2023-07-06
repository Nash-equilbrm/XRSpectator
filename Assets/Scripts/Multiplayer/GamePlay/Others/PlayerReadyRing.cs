using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReadyRing : MonoBehaviour
{
    public Player m_player;
    public GameObject m_readyRing;
    public GameObject m_notReadyRing;
    public GameObject m_attackRing;



    private void Start()
    {
        transform.SetParent(null);
    }
    void Update()
    {
        if (m_player.IsReady)
        {
            if (m_player.IsMyTurn && m_player.CurrentMonster != null &&  m_player.CurrentMonster.OnAttack)
            {
                m_readyRing.SetActive(false);
                m_notReadyRing.SetActive(false);
                m_attackRing.SetActive(true);
            }
            else if(m_player.IsMyTurn)
            {
                m_readyRing.SetActive(true);
                m_notReadyRing.SetActive(false);
                m_attackRing.SetActive(false);
            }
            else if (!m_player.IsMyTurn)
            {
                m_readyRing.SetActive(false);
                m_notReadyRing.SetActive(true);
                m_attackRing.SetActive(false);
            }
        }

        
    }
}
