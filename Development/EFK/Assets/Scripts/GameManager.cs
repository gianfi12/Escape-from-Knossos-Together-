﻿using UnityEngine;
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
    [SerializeField] private bool isDebug;
    [SerializeField] private int seed;

    [SerializeField] private GameObject navMesh;

    void Start()
    {
        BeginGame();
        EventManager.StartListening(EventType.FinishGame,new UnityAction(FinishGame));
    }

    void FinishGame()
    {
        if (_playerInstanceLocal.IsDead || _playerInstanceRemote.IsDead)
        {
            _cameraInstance.m_Follow = null;
            _playerInstanceLocal.FinishGame(false);

        }
    }

    public void RestartGame()
    {
        _playerInstanceLocal.GetComponent<PhotonView>().RPC("ReloadMain", RpcTarget.All);
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
            if (isDebug)
            {
                if(seed==0) seed = UnityEngine.Random.Range(0, 10000);
                Debug.Log("The seed is "+seed+"\n");
            }
            else
            {
                seed = UnityEngine.Random.Range(0, 10000);
            }
            _levelMap.Seed = seed;
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

    public void SetRemotePlayer()
    {
        GameObject [] players = GameObject.FindGameObjectsWithTag("Player");
        if (players[0] == _playerInstanceLocal.gameObject)
        {
            _playerInstanceRemote = players[1].GetComponent<PlayerControllerMap>();
            _playerInstanceRemote.GetComponent<Animator>().runtimeAnimatorController = _playerInstanceRemote.RuntimeAnimators[(_playerInstanceRemote.GetComponent<PhotonView>().ViewID / 1000) - 1];
        }
        else
        {
            _playerInstanceRemote = players[0].GetComponent<PlayerControllerMap>();
            _playerInstanceRemote.GetComponent<Animator>().runtimeAnimatorController = _playerInstanceRemote.RuntimeAnimators[(_playerInstanceRemote.GetComponent<PhotonView>().ViewID / 1000) - 1];
        }
        EventManager.StartListening(EventType.FinishGame,new UnityAction(FinishGame));
    }
    
}
