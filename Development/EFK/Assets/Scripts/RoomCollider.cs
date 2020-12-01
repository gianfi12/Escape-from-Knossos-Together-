using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomCollider : MonoBehaviour
{
    [SerializeField] private RoomAbstract room;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            PlayerControllerMap player;
            GameObject[] playersList = GameObject.FindGameObjectsWithTag("Player");

            if (PhotonNetwork.IsConnected) {
                player = playersList.Where(x => !x.GetComponent<PhotonView>().IsMine).First().GetComponent<PlayerControllerMap>();
            }
            else {
                player = playersList[0].GetComponent<PlayerControllerMap>();
            }
        
            player.SetMyRoom(room);
            room.Player = player;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            room.Player = null;
        }
    }

    public RoomAbstract Room
    {
        get => room;
        set => room = value;
    }
}
