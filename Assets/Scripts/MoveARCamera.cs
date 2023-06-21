using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveARCamera : MonoBehaviour
{
    public Transform ARCamera;
    public Transform Head;

    // Update is called once per frame
    void Update()
    {
        if(ARCamera != null)
        {
            if (GetComponent<PhotonView>().IsMine)
            {
                transform.position = ARCamera.position;
                Vector3 euler = ARCamera.eulerAngles;
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, euler.y, transform.eulerAngles.z);
                Head.eulerAngles = new Vector3(euler.x, Head.eulerAngles.y, euler.z);
            }
        }
    }
}
