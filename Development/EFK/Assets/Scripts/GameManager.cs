using UnityEngine;
using Photon.Pun;
using Cinemachine;
using System;

public class GameManager : MonoBehaviour
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
        _levelMap = Instantiate(levelPrefab);
        _levelMap.CreateMap();

        if (PhotonNetwork.IsConnected) _playerInstance = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<PlayerControllerMap>();
        else _playerInstance = Instantiate(playerPrefab).GetComponent<PlayerControllerMap>();

        _levelMap.PlacePlayer(_playerInstance);
        
        _cameraInstance = Instantiate(mainCamera);
        _cameraInstance.m_Follow = _playerInstance.transform;
    }
}
