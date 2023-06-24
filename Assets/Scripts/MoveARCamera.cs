using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveARCamera : MonoBehaviour
{
    public Transform ARCamera;
    public Vector3 offset;
    void Update()
    {
        if(ARCamera != null && GetComponent<PhotonView>().IsMine)
        {
            transform.position = ARCamera.position + offset;
            transform.rotation = ARCamera.rotation;
        }
        

    }
}
