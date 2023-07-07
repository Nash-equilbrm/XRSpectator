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

        if (isFound) {
            QuadObj.transform.position = player[0].transform.position + new Vector3(0, firstPersonViewOffset, 0);
            QuadObj.transform.LookAt(leftEye);
            QuadObj2.transform.position = player[1].transform.position + new Vector3(0, firstPersonViewOffset, 0);
            QuadObj2.transform.LookAt(leftEye);
        }
    }
}
