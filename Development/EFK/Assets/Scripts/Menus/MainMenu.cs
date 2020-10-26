using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _findOpponetPanel = null;
    [SerializeField] private GameObject _waitingStatusPanel = null;
    [SerializeField] private Text _waitingStatusText = null;

    private bool _isConnecting = false;
    private const string Gameversion = "0.1";
    private const int MaxPlayerPerRoom = 2;

    private void Awake()
    {
        //Set that anytime I switch a scene I do it for all the players
        PhotonNetwork.AutomaticallySyncScene = true;  
    }

    public void FindOpponent()
    {
        _isConnecting = true;
        
        _findOpponetPanel.SetActive(false);
        _waitingStatusPanel.SetActive(true);
        _waitingStatusText.text = "Searching...";

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = Gameversion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");

        if (_isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        _waitingStatusPanel.SetActive(false);
        _findOpponetPanel.SetActive(true);
        
        Debug.Log($"Disconnected due to: {cause}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No clients are waiting, creating a new room...");

        PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = MaxPlayerPerRoom});
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Client succesfully joined a room");

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount != MaxPlayerPerRoom)
        {
            _waitingStatusText.text = "Waiting For Opponent";
            Debug.Log("Client is waiting for an opponent");
        }
        else
        {
            _waitingStatusText.text = "Opponent found";
            Debug.Log("Match is ready to begin");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayerPerRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            _waitingStatusText.text = "Opponent found";
            Debug.Log("Match is ready to begin");
            
            PhotonNetwork.LoadLevel("Main");
        }
    }
}
