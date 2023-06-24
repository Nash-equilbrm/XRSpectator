using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.Management;
using Vuforia;

public partial class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance = null;

    [Header("User")]
    public bool isAudience;
    public Player playerManager;
    public GameObject playerAvatarPrefab;


    [Header("Hololens")]
    public GameObject ARCamera;
    public Transform imageTarget;
    public GameObject firstPersonViewStreamCamera;
  

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
                UpdateZed();
            }
            else 
            {
                // update for hololens user to track marker, stop tracking after x(secs) and go to gameplay scene
                if (!TrackedWithVuforia)
                {
                    UpdateHololens();
                }
                // start gameplay for hololens user
                else
                {
                    UpdateGameplay();
                }
            }
        }
    }


   
}