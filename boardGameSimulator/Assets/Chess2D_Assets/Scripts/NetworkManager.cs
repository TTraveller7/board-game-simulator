using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private SettingsUI uiManager;
    private const int MAX_PLAYERS = 2;
    private const string TEAM = "team";

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true; 
    }

    private void Update()
    {
        uiManager.SetConnectionStatus(PhotonNetwork.NetworkClientState.ToString());
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.LogError($"Connected to server. Looking for random room.");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError($"Joining random room failed because of {message}. Creating a new one.");
        PhotonNetwork.CreateRoom(null);
    }

    private void PrepareTeamSelectionoptions()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            var firstPlayer = PhotonNetwork.CurrentRoom.GetPlayer(1);
            if (firstPlayer.CustomProperties.ContainsKey(TEAM))
            {
                var occupiedTeam = firstPlayer.CustomProperties[TEAM];
                uiManager.RestrictTeamChoice((string)occupiedTeam);
            }
        }
        
    }

    public override void OnJoinedRoom()
    {
        Debug.LogError($"Player {PhotonNetwork.LocalPlayer.ActorNumber} joined the room.");
        PrepareTeamSelectionoptions();
        uiManager.ShowTeamSelectionScreen();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogError($"Player {newPlayer.ActorNumber} entered the room.");
    }

    internal void SelectTeam(int team)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { TEAM, team } });
    }

    internal bool IsRoomFull()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
    }
}
