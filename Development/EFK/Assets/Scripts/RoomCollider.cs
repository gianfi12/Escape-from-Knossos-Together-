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
            
            if (PhotonNetwork.IsConnected) {
                // If connected, set room to own player only when room is entered/triggered by other player
                if (!other.GetComponent<PhotonView>().IsMine) {
                    GameObject[] playersList = GameObject.FindGameObjectsWithTag("Player");
                    player = playersList.Where(x => x.GetComponent<PhotonView>().IsMine).First().GetComponent<PlayerControllerMap>();
                    player.SetRoom(room);
                    room.Player = player;
                }
                else
                {
                    other.GetComponent<PlayerControllerMap>().ResetInventory();
                }
            }
            else {
                player = other.GetComponent<PlayerControllerMap>();
                player.SetRoom(room);
                room.Player = player;
            }
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
