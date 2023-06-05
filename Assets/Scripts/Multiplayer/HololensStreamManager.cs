using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{
    [Header("First person streaming")]
    public GameObject m_WebRTCStreaming;

    public GameObject m_HololensFirstPersonStreaming;
    public GameObject m_HololensFirstPersonLocalFeed;



    public void StartStreaming()
    {
        m_WebRTCStreaming?.SetActive(true);
        m_HololensFirstPersonStreaming?.SetActive(true);
        m_HololensFirstPersonLocalFeed?.SetActive(true);
    }
}
