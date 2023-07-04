using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFieldsMovement : MonoBehaviour
{
    public Player m_player;
    public float m_rotateDuration;
    public Vector3 m_offset;
    public PhotonView m_photonView;
    private void Start()
    {
        m_photonView = GetComponent<PhotonView>();
        if (m_photonView.IsMine)
        {
            gameObject.transform.SetParent(null);
        }

        if (m_player.PlayerID == 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
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
        Quaternion targetRotation = Quaternion.Euler(0f, eulerY, 0f);

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
