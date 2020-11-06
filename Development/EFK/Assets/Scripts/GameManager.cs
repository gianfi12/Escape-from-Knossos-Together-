using UnityEngine;
using Photon.Pun;
using Cinemachine;
using System.Collections;
using Random = System.Random;
using System.Linq;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelMap levelPrefab;
    private LevelMap _levelMap;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject commonInfoPrefab;
    private GameObject commonInfo;
    private PlayerControllerMap _playerInstance;
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    private CinemachineVirtualCamera _cameraInstance;
    
    private int mapSeed; 

    void Start()
    {
        BeginGame();
    }


    [PunRPC]
    private void GenerateSeed() {
        mapSeed = UnityEngine.Random.Range(0, 10000);
    }

    private void BeginGame()
    {
        _levelMap = Instantiate(levelPrefab);

        if (PhotonNetwork.IsConnected) {
            if (PhotonNetwork.IsMasterClient) {
                PhotonView photonView = PhotonView.Get(this);
                photonView.RPC("GenerateSeed", RpcTarget.All);
            }
        }
        else {
            _levelMap.Seed = UnityEngine.Random.Range(0, 10000);
        }
        
        _levelMap.CreateMap();
        
        if (PhotonNetwork.IsConnected) {
            _playerInstance = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<PlayerControllerMap>();
            int viewID = _playerInstance.GetComponent<PhotonView>().ViewID;
            _levelMap.PlacePlayer(_playerInstance, viewID/1000);
        }else {
            _playerInstance = Instantiate(playerPrefab).GetComponent<PlayerControllerMap>();
            _levelMap.PlacePlayer(_playerInstance, 1);
        }
        _cameraInstance = Instantiate(mainCamera);
        _cameraInstance.m_Follow = _playerInstance.transform;
    }
}
