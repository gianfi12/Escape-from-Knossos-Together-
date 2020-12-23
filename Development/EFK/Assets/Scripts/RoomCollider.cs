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
                    player = other.GetComponent<PlayerControllerMap>();
                    player.ResetInventory();
                    // player.SetTimer(room.MaxTimeInSeconds, room.TimeoutTriggersLoss);
                    player.IncrementTimer(room.TimeIncrementInSeconds);
                }
            }
            else {
                player = other.GetComponent<PlayerControllerMap>();
                player.SetRoom(room);
                room.Player = player;
                // player.SetTimer(room.MaxTimeInSeconds, room.TimeoutTriggersLoss);
                player.IncrementTimer(room.TimeIncrementInSeconds);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        /* SET OFF TIMER WHEN EXITING ROOM 
         
        if (other.CompareTag("Player")) {
            if (!PhotonNetwork.IsConnected || other.GetComponent<PhotonView>().IsMine) {
                other.GetComponent<PlayerControllerMap>().SetTimer(0, false);
            }
            room.Player = null;
        }*/
    }

    public RoomAbstract Room
    {
        get => room;
        set => room = value;
    }
}
