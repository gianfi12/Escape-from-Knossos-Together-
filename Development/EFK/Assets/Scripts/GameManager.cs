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
                _levelMap.Seed = UnityEngine.Random.Range(0, 10000);
                _levelMap.CreateMap();
                this.photonView.RPC("SetLevelMap", RpcTarget.Others, _levelMap);
            }
        }
        else {
            _levelMap = Instantiate(levelPrefab);
            _levelMap.Seed = UnityEngine.Random.Range(0, 10000);
            _levelMap.CreateMap();
        }
   
        
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

    [PunRPC]
    public void SetLevelMap(LevelMap levelMap) {
        this._levelMap = levelMap;
    }
}
