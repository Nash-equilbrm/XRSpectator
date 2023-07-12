using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.Management;
public partial class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance = null;

    [Header("User")]
    public bool isAudience;
    public Player playerManager;
    public GameObject player0AvatarPrefab;
    public GameObject player1AvatarPrefab;
    public GameObject pleaseLookAtMarkerTxt;


    [Header("Hololens")]
    public GameObject ARCamera;
    public Transform imageTarget;
    public Camera firstPersonViewStreamCamera;


    [Header("ZedCamera")]
    public Transform zedCameraTransform;
    public GameObject marker;
    public GameObject zedRigStereo;
    public GameObject zedCaptureToOpenCV;
    public GameObject ArUcoDetectManager;
    private bool m_userRoleInitialized = false;
    private void Update()
    {
        if (m_userRoleInitialized)
        {
            // update for zed users (audiences), keep tracking marker every frames
            if (isAudience)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    overlayMode = !overlayMode;
                }

                // change player 1st view
                if (Input.GetKeyDown(KeyCode.S))
                {
                    firstViewSwap = !firstViewSwap;
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    ResetSocketServer();
                }
                if (Input.GetKeyDown(KeyCode.P))
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    players[0].GetComponent<Player>().TurnOnWebCamTexture(true);
                    //players[1].GetComponent<Player>().TurnOnWebCamTexture(true);

                }
                if (Input.GetKeyDown(KeyCode.O))
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    players[0].GetComponent<Player>().TurnOnWebCamTexture(false);
                    //players[1].GetComponent<Player>().TurnOnWebCamTexture(false);

                }
                UpdateZed();
                UpdateFirstPersonView();
            }
            else
            {
                // update for hololens user to track marker, stop tracking after x(secs) and go to gameplay scene
                UpdateHololens();
            }
        }
    }



}