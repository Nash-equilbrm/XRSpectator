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
    public Transform m_rotationControl;

    private float m_originYEuler = -1;
    private void Start()
    {
        m_photonView = GetComponent<PhotonView>();
        gameObject.transform.SetParent(null);
    }


    private bool m_initRotation = false;
    void Update()
    {
        if (m_photonView.IsMine && m_player)
        {
            transform.position = m_player.transform.position + m_offset;

            if(m_player.Opponent != null)
            {
                Vector3 opponentPosition = m_player.Opponent.transform.position;
                transform.LookAt(new Vector3(opponentPosition.x, transform.position.y, opponentPosition.z));
            }
            else
            {
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }
    }


    public void Rotate(float eulerY)
    {
        StartCoroutine(RotateCoRoutine(eulerY));
    }


    private IEnumerator RotateCoRoutine(float eulerY)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = m_rotationControl.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0f, eulerY, 0f);

        while (elapsedTime < m_rotateDuration)
        {
            float t = elapsedTime / m_rotateDuration;
            m_rotationControl.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        m_rotationControl.localRotation = targetRotation;
    }




}
