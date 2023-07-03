using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReadyRing : MonoBehaviour
{
    public Player m_player;
    public Renderer m_renderer;

    private void Start()
    {
        transform.SetParent(null);
    }
    void Update()
    {
        if (!m_player.IsReady)
        {
            return;
        }

        if (m_renderer)
        {
            if(m_player.IsMyTurn)
            {
                m_renderer.material.SetColor("_Color", Color.green);
            }
            else
            {
                m_renderer.material.SetColor("_Color", Color.red);
            }
        }
    }
}
