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
    // public GameObject pleaseLookAtMarkerTxt;


    [Header("Hololens")]
    // public GameObject ARCamera;
    // public Transform imageTarget;
    // public GameObject firstPersonViewStreamCamera;


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
            UpdateZed();

        }
    }



}