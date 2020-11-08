using UnityEngine;
using Photon.Pun;
using Cinemachine;
using System.Collections;
using Random = System.Random;
using System.Linq;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviourPun
{
    [SerializeField] private LevelMap levelPrefab;
    private LevelMap _levelMap;
    [SerializeField] private GameObject playerPrefab;
    private PlayerControllerMap _playerInstance;
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    private CinemachineVirtualCamera _cameraInstance;


    void Start()
    {
        BeginGame();
    }

    private void BeginGame()
    {
        if (PhotonNetwork.IsConnected) {
            if (PhotonNetwork.IsMasterClient) {
                _levelMap =  PhotonNetwork.Instantiate(levelPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<LevelMap>();
                _levelMap.CreateMapOverNetwork();
                _levelMap.InstantiatePlayersOverNetwork();
            }
        }
        else {
            _levelMap = Instantiate(levelPrefab);
            _levelMap.Seed = UnityEngine.Random.Range(0, 10000);
            _levelMap.CreateMap();
        }
   
        if (!PhotonNetwork.IsConnected) {
            _playerInstance = Instantiate(playerPrefab).GetComponent<PlayerControllerMap>();
            _cameraInstance = Instantiate(mainCamera);
            _cameraInstance.m_Follow = _playerInstance.transform;
            _levelMap.PlacePlayer(_playerInstance, 1);
        }
    }

    public void SetPlayerInstance(PlayerControllerMap playerInstance) {
        _playerInstance = playerInstance;
        _cameraInstance = Instantiate(mainCamera);
        _cameraInstance.m_Follow = _playerInstance.transform;
    }
}
