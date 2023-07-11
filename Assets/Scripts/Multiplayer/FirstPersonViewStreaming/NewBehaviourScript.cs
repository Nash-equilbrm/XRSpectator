using System;
using System.Collections;
using UnityEngine;

public partial class GameManager
{
    [Header("First Person View")]
    public GameObject QuadObj;
    public GameObject QuadObj2;
    private GameObject[] player;
    public float firstPersonViewOffset;
    private bool isFound = false;
    private bool overlayMode = false;
    private bool firstViewSwap = true;
    public GameObject overlayViews;

    void UpdateFirstPersonView()
    {
        if (!isFound)
        {
            player = GameObject.FindGameObjectsWithTag("Player");
            if (player != null && player.Length == 2)
            {
                isFound = true;
            }
        }

        if (isFound && !overlayMode)
        {
            overlayViews.SetActive(false);

            if (firstViewSwap)
            {
                QuadObj.transform.SetParent(null);
                QuadObj.transform.position = player[0].transform.position + new Vector3(0, firstPersonViewOffset, 0);
                QuadObj.transform.LookAt(leftEye);

                QuadObj2.transform.position = player[1].transform.position + new Vector3(0, firstPersonViewOffset, 0);
                QuadObj2.transform.LookAt(leftEye);
            }
            else
            {
                QuadObj.transform.SetParent(null);
                QuadObj.transform.position = player[1].transform.position + new Vector3(0, firstPersonViewOffset, 0);
                QuadObj.transform.LookAt(leftEye);

                QuadObj2.transform.position = player[0].transform.position + new Vector3(0, firstPersonViewOffset, 0);
                QuadObj2.transform.LookAt(leftEye);
            }
        }

        if (isFound && overlayMode)
        {
            overlayViews.SetActive(true);
        }

    }


    private void ResetSocketServer()
    {
        StartCoroutine(ResetSocketServerCoroutine());
    }

    private IEnumerator ResetSocketServerCoroutine()
    {
        socketManager.enabled = false;
        yield return null;
        socketManager.enabled = true;

    }
}