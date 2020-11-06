using UnityEngine;
using Photon.Pun;
using Cinemachine;
using System;
using Random = System.Random;

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
    
    void Start()
    {
        BeginGame();
    }

    private void BeginGame()
    {
        _levelMap = Instantiate(levelPrefab);
        if (PhotonNetwork.IsMasterClient)
        {
            commonInfo = PhotonNetwork.Instantiate(commonInfoPrefab.name, Vector3.zero, Quaternion.identity);
            commonInfo.GetComponent<MapSeedInfo>().GenerateMapSeed();
        }
        else
        {
            do
            {
                commonInfo = GameObject.Find("CommonInfo");
            } while (commonInfo == null);

        }

        _levelMap.Seed = commonInfo.GetComponent<MapSeedInfo>().GetSeed();
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
