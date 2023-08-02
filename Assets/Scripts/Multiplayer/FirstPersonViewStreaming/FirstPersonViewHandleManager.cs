using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public partial class GameManager
{
    [Header("First Person View")]
    public Renderer[] playerFPVs;
    public RawImage[] playerOverlayFPVs;

    public GameObject overlayViews;
    public VirtualAndRealSceneBlend[] fpvFrameHandler;
    private List<Player> players = new List<Player>();
    private bool playersJoined = false;
    private bool overlayMode = false;


    void UpdateAudienceInputs()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            overlayMode = !overlayMode;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SwapFPV();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSocketServer();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            for(int i = 0; i < players.Count; ++i)
            {
                players[i].TurnOnWebCamTexture(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            for (int i = 0; i < players.Count; ++i)
            {
                players[i].TurnOnWebCamTexture(true);
            }

        }
    }

    private void SwapFPV()
    {
        for(int i = 0; i < players.Count; ++i)
        {
            if(i == players.Count - 1)
            {
                playerFPVs[i].transform.parent.GetComponent<FollowTransform>().follow = players[0].transform;
            }
            else
            {
                playerFPVs[i].transform.parent.GetComponent<FollowTransform>().follow = players[i + 1].transform;
            }
        }
    }

    void UpdateFirstPersonView()
    {
        if (!playersJoined)
        {
            GameObject[] playerObjs = GameObject.FindGameObjectsWithTag("Player");
            if (playerObjs != null && playerObjs.Length == 2)
            {
                for(int i = 0; i < playerObjs.Length; ++i)
                {
                    players.Add(playerObjs[i].GetComponent<Player>());
                    FollowTransform followTransform = playerFPVs[i].transform.parent.GetComponent<FollowTransform>();
                    if (followTransform)
                    {
                        followTransform.follow = playerObjs[i].transform;
                    }
                }
                playersJoined = true;
            }
        }
        else
        {
            if (overlayMode)
            {
                overlayViews.SetActive(true);
                for (int i = 0; i < playerOverlayFPVs.Length; ++i)
                {
                    playerOverlayFPVs[i].texture = fpvFrameHandler[i].DstTexture;

                }
            }
            else
            {
                overlayViews.SetActive(false);
                for (int i = 0; i < playerFPVs.Length; ++i)
                {
                    playerFPVs[i].transform.parent.LookAt(leftEye);
                    playerFPVs[i].material.mainTexture = fpvFrameHandler[i].DstTexture;

                }

            }

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