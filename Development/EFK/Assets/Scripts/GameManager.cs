using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelMap levelPrefab;
    private LevelMap _levelMap;
    [SerializeField] private PlayerController playerPrefab;
    private PlayerController _playerInstance;
    [SerializeField] private GameObject mainCamera;

    void Start()
    {
        BeginGame();
    }

    private void BeginGame()
    {
        _levelMap = Instantiate(levelPrefab);
        _levelMap.CreateMap();
        _playerInstance = Instantiate(playerPrefab);
        _levelMap.PlacePlayer(_playerInstance);
        Instantiate(mainCamera);
    }
}
