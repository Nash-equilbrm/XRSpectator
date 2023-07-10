using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{
    [Header("First Person View")]
    public GameObject QuadObj;
    public GameObject QuadObj2;
    private GameObject[] player;
    public float firstPersonViewOffset;

    private bool isFound = false;
    private bool overlayMode = true;

    void UpdateFirstPersonView()
    {
        if (!isFound)
        {
            player = GameObject.FindGameObjectsWithTag("Player");
            if (player != null && player.Length == 1)
            {
                isFound = true;
            }
        }

        if (isFound && overlayMode)
        {
            QuadObj.transform.SetParent(null);
            QuadObj.transform.position = player[0].transform.position + new Vector3(0, firstPersonViewOffset, 0);
            QuadObj.transform.LookAt(leftEye);

            // QuadObj2.transform.position = player[1].transform.position + new Vector3(0, firstPersonViewOffset, 0);
            // QuadObj2.transform.LookAt(leftEye);
        }

        if (isFound && !overlayMode)
        {
            QuadObj.transform.SetParent(leftEye);
            QuadObj.transform.localPosition = new Vector3(0, 0, 1);
            QuadObj.transform.localRotation = Quaternion.identity;
            // QuadObj2.transform.SetParent(leftEye);
            // QuadObj2.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
}
