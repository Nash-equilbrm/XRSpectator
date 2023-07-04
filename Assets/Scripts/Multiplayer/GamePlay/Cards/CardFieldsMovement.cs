using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFieldsMovement : MonoBehaviour
{
    public Player m_player;
    public float m_rotateDuration;
    public Vector3 m_offset;
    public PhotonView m_photonView;

    private float m_originYEuler = -1;
    private void Start()
    {
        m_photonView = GetComponent<PhotonView>();
        gameObject.transform.SetParent(null);

        if (m_player.PlayerID == 0)
        {
            Debug.Log("Init card fields rotation for player 0");
            m_originYEuler = 0;
        }
        else if (m_player.PlayerID == 1)
        {
            Debug.Log("Init card fields rotation for player 1");
            m_originYEuler = 180;
        }
        transform.eulerAngles = new Vector3(0, m_originYEuler, 0);
    }

    

    void Update()
    {
        if (m_photonView.IsMine && m_player)
        {
            transform.position = m_player.transform.position + m_offset;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }


    public void Rotate(float eulerY)
    {
        StartCoroutine(RotateCoRoutine(eulerY));
    }


    private IEnumerator RotateCoRoutine(float eulerY)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0f,m_originYEuler + eulerY, 0f);

        while (elapsedTime < m_rotateDuration)
        {
            float t = elapsedTime / m_rotateDuration;
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = targetRotation;
    }




}
