using UnityEngine;
using Photon.Pun;
using Cinemachine;
using System.Collections;
using Random = System.Random;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPun
{
    [SerializeField] private LevelMap levelPrefab;
    private LevelMap _levelMap;
    [SerializeField] private GameObject playerPrefab;
    private PlayerControllerMap _playerInstanceLocal;
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    private CinemachineVirtualCamera _cameraInstance;

    [SerializeField] private GameObject navMesh;

    void Start()
    {
        BeginGame();
    }

    void FinishGame()
    {
        if (_playerInstanceLocal.IsDead)
        {
            _cameraInstance.m_Follow = null;
            _playerInstanceLocal.FinishGame();
        }
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
            navMesh.GetComponent<NavMeshSurface2d>().BuildNavMesh();

            _playerInstanceLocal = Instantiate(playerPrefab).GetComponent<PlayerControllerMap>();
            _cameraInstance = Instantiate(mainCamera);
            _cameraInstance.m_Follow = _playerInstanceLocal.transform;
            _levelMap.PlacePlayer(_playerInstanceLocal, 1);
            _playerInstanceLocal.SetGameManager(this);
        }
    }

    public void SetPlayerInstance(PlayerControllerMap playerInstance) {
        _playerInstanceLocal = playerInstance;
        _cameraInstance = Instantiate(mainCamera);
        _cameraInstance.m_Follow = _playerInstanceLocal.transform;
        
        navMesh.GetComponent<NavMeshSurface2d>().BuildNavMesh();
    }
    
}
