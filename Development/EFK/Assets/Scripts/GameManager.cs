using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelMap levelPrefab;
    private LevelMap _levelMap;
    [SerializeField] private PlayerControllerMap playerPrefab;
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
        _playerInstance = Instantiate(playerPrefab);
        _levelMap.PlacePlayer(_playerInstance);
        _cameraInstance = Instantiate(mainCamera);
        _cameraInstance.m_Follow = _playerInstance.transform;
    }
}
