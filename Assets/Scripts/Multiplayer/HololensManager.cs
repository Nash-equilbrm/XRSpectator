using Microsoft.MixedReality.Toolkit;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Vuforia;

public partial class GameManager
{
    private float m_trackMarkerDuration = 3f;
    private float m_trackMarkerCount = 0f;
    public bool HololensMarkerTracked { get => m_hololensMarkerTracked; set => m_hololensMarkerTracked = value; }
    private bool m_hololensMarkerTracked = false;

    private Transform m_ARPlaySpace;

    public void InitHololens()
    {
        m_ARPlaySpace = ARCamera.transform.parent;
        ARCamera.transform.localPosition = Vector3.zero;
        ARCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;

        m_userRoleInitialized = true;
    }

    public bool TrackedWithVuforia { get => m_trackedWithVuforia; }
    private bool m_trackedWithVuforia = false;

    public void UpdateHololens()
    {
        if (!TrackedWithVuforia)
        {
            if (m_trackMarkerCount >= m_trackMarkerDuration)
            {
                TurnOffVuforia();
                ARCamera.GetComponent<TrackedPoseDriver>().enabled = true;


                if (inGameAudience)
                {
                    CreatePlayerAvatar(-1);
                }

                else if (firstPersonViewStreaming)
                {
                    StartStreaming();
                }
                
                m_trackedWithVuforia = true;

            }
            else
            {
                if (HololensMarkerTracked)
                {
                    m_trackMarkerCount += Time.deltaTime;
                }
                else
                {
                    m_trackMarkerCount = 0;
                }
            }
        }
        else
        {
            // start gameplay for hololens user
            if (!inGameAudience)
            {
                UpdateGameplay();
            }
        }

    }

   
    private void TurnOffVuforia()
    {
        Debug.Log("Turn off vuforia");
        ARCamera.GetComponent<VuforiaBehaviour>().enabled = false;

        ARCamera.transform.SetParent(imageTarget);
        imageTarget.transform.position = Vector3.zero;
        imageTarget.transform.rotation = Quaternion.identity;



        m_ARPlaySpace.position = ARCamera.transform.position;
        m_ARPlaySpace.rotation = ARCamera.transform.rotation;

        ARCamera.transform.SetParent(m_ARPlaySpace);
        ARCamera.transform.localPosition = Vector3.zero;
        ARCamera.transform.localRotation = Quaternion.identity;

      
    }



    public Player CreatePlayerAvatar(int playerID)
    {
        if (playerID == -1)
        {
            GameObject playerModel = null;
            playerModel = PhotonNetwork.Instantiate("Prefabs/Audience", ARCamera.transform.position, ARCamera.transform.rotation);


            if (playerModel != null && playerModel.GetComponent<PhotonView>().IsMine)
            {
                playerModel.GetComponent<MoveARCamera>().ARCamera = ARCamera.transform;
                return null;
            }
        }
        else
        {
            GameObject playerModel = null;
            if (playerID == 0)
            {
                playerModel = PhotonNetwork.Instantiate("Prefabs/" + player0AvatarPrefab.name, ARCamera.transform.position, ARCamera.transform.rotation);
            }
            else if(playerID == 1)
            {
                playerModel = PhotonNetwork.Instantiate("Prefabs/" + player1AvatarPrefab.name, ARCamera.transform.position, ARCamera.transform.rotation);
            }


            if (playerModel != null && playerModel.GetComponent<PhotonView>().IsMine)
            {
                playerModel.GetComponent<MoveARCamera>().ARCamera = ARCamera.transform;
                Player playerManager = playerModel.GetComponent<Player>();
                playerManager.SetPlayerID(playerID);

                return playerManager;
            }
        }
        Debug.Log("Error create player");
        return null;
    }
}
