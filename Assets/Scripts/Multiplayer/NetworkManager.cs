using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public partial class GameManager
{
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.UseRpcMonoBehaviourCache = true;
        PhotonNetwork.EnableCloseConnection = true;
    }

    private void OnApplicationQuit()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.EmptyRoomTtl = 0;
            PhotonNetwork.CurrentRoom.PlayerTtl = 0;

            //foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            //{
            //    if (!player.IsMasterClient)
            //    {
            //        PhotonNetwork.CloseConnection(player);
            //        PhotonNetwork.SendAllOutgoingCommands();
            //    }
            //}
        }
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        if (!PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinOrCreateRoom("selabi87", new RoomOptions { IsOpen = true, IsVisible = true, MaxPlayers = 0 }, null, null);
            Debug.Log("onJoinRoom");
        }
    }

    public override void OnDisconnected(DisconnectCause disconnectCause)
    {
        base.OnDisconnected(disconnectCause);
        Application.Quit();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Application.Quit();
    }

    [Header("Network")]
    public string m_serverIP;
    public FMSocketIOManager socketManager;

    public override void OnJoinedRoom()
    {
        if (isAudience)
        {
           PhotonNetwork.CurrentRoom.SetCustomProperties(
                new ExitGames.Client.Photon.Hashtable
                {
                    { "SERVERIP", m_serverIP } 

                }


            );

        }
        else
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("SERVERIP", out var SERVERIP))
            {
                m_serverIP = (string)SERVERIP;
                socketManager.Action_SetIP(m_serverIP);
            }
        }



        if (isAudience)
        {
            InitZED();
        }
        else
        {
            InitHololens();
        }
    }

}
