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
    private PlayerControllerMap _playerInstanceRemote;
    [SerializeField] private CinemachineVirtualCamera mainCamera;
    private CinemachineVirtualCamera _cameraInstance;

    [SerializeField] private GameObject navMesh;

    void Start()
    {
        BeginGame();

    }

    void FinishGame()
    {
        if (_playerInstanceLocal.IsDead && _playerInstanceRemote.IsDead)
        {
            //TODO end game
        }else if (_playerInstanceLocal.IsDead)
        {
            _cameraInstance.m_Follow = _playerInstanceRemote.transform;
            _playerInstanceLocal.gameObject.SetActive(false);
            //check if online you can control the second player(it shouldn't be)
        }
        //the other case is not needed it will be change the camera on the remote side
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

            Instantiate(playerPrefab);
            _playerInstanceLocal = Instantiate(playerPrefab).GetComponent<PlayerControllerMap>();
            _cameraInstance = Instantiate(mainCamera);
            _cameraInstance.m_Follow = _playerInstanceLocal.transform;
            _levelMap.PlacePlayer(_playerInstanceLocal, 1);
            _playerInstanceLocal.SetGameManager(this);
            SetRemotePlayer();
        }
    }

    public void SetPlayerInstance(PlayerControllerMap playerInstance) {
        _playerInstanceLocal = playerInstance;
        _cameraInstance = Instantiate(mainCamera);
        _cameraInstance.m_Follow = _playerInstanceLocal.transform;
        
        navMesh.GetComponent<NavMeshSurface2d>().BuildNavMesh();
        
        SetRemotePlayer();
    }

    public void SetRemotePlayer()
    {
        GameObject [] players = GameObject.FindGameObjectsWithTag("Player");
        if (players[0] == _playerInstanceLocal.gameObject)
        {
            _playerInstanceRemote = players[1].GetComponent<PlayerControllerMap>();
        }
        else
        {
            _playerInstanceRemote = players[0].GetComponent<PlayerControllerMap>();
        }
        EventManager.StartListening(EventType.FinishGame,new UnityAction(FinishGame));
    }
    
}
