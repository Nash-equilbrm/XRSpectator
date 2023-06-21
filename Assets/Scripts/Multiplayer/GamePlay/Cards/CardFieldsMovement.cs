using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFieldsMovement : MonoBehaviour
{
    public int id;
    public Transform m_playerTransform;
    public float m_rotateDuration;
    public Vector3 m_offset;
    public Transform[] m_cardDisplayTransforms;

    // Update is called once per frame
    void Update()
    {
        if (m_playerTransform == null && GameManager.Instance.playerManager != null && GameManager.Instance.playerManager.PlayerID == id)
        {
            m_playerTransform = GameManager.Instance.playerManager.transform;
            GameManager.Instance.cardDisplays = this;
        }
        if(m_playerTransform)
        {
            transform.position = m_playerTransform.position + m_offset;
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
