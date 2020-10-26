using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate(_playerPrefab.name, Vector3.zero, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
