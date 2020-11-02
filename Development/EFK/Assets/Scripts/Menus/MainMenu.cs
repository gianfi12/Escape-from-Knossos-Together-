using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _findGamePanel = null;
    [SerializeField] private GameObject _waitingStatusPanel = null;
    [SerializeField] private Text _waitingStatusText = null;
    [SerializeField] private Text _waitingStatusRoom = null;
    [SerializeField] private Text _createRoomName = null;
    [SerializeField] private Text _selectedRoomName = null;
    
    private const string Gameversion = "0.1";
    private const int MaxPlayerPerRoom = 2;

    private void Awake()
    {
        //Set that anytime I switch a scene I do it for all the players
        PhotonNetwork.AutomaticallySyncScene = true;  
        
        PhotonNetwork.GameVersion = Gameversion;
        PhotonNetwork.ConnectUsingSettings();
    }

    private void VerifyMasterConnection()
    {
        if (PhotonNetwork.IsConnected) return;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        VerifyMasterConnection();
        if (_createRoomName.text.Length != 0)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = MaxPlayerPerRoom;
            PhotonNetwork.JoinOrCreateRoom(_createRoomName.text, roomOptions, TypedLobby.Default);
        }
    }

    public void JoinSelectedRoom()
    {
        VerifyMasterConnection();
        if (_selectedRoomName.text.Length != 0) PhotonNetwork.JoinRoom(_selectedRoomName.text);
        }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room creation failed "+message);
    }

    public void JoinRandomRoom()
    {
        VerifyMasterConnection();
        
        _findGamePanel.SetActive(false);
        _waitingStatusPanel.SetActive(true);
        _waitingStatusText.text = "Searching...";
        
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        _waitingStatusPanel.SetActive(false);
        _findGamePanel.SetActive(true);
        
        Debug.Log($"Disconnected due to: {cause}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No clients are waiting, creating a new room...");

        //DA AGGIUNGERE NOME CASUALE
        PhotonNetwork.CreateRoom( "000", new RoomOptions {MaxPlayers = MaxPlayerPerRoom});
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Client succesfully joined a room:"+PhotonNetwork.CurrentRoom.Name);
        
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerCount != MaxPlayerPerRoom)
        {
            _findGamePanel.SetActive(false);
            _waitingStatusPanel.SetActive(true);
            _waitingStatusRoom.text = "ROOM NAME: " + PhotonNetwork.CurrentRoom.Name;
            _waitingStatusText.text = "Waiting for another player...";
            Debug.Log("Client is waiting for another player");
        }
        else
        {
            _waitingStatusText.text = "Player found";
            Debug.Log("Match is ready to begin");
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join Room failed "+message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayerPerRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            _waitingStatusText.text = "Player found";
            Debug.Log("Match is ready to begin");
            
            PhotonNetwork.LoadLevel("Main");
        }
    }
}
