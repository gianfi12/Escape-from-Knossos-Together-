using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCollider : MonoBehaviour
{
    [SerializeField] private RoomAbstract room;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControllerMap player = other.GetComponent<PlayerControllerMap>();
            player.SetRoomManager(room);
            room.Player = player; 
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        room.Player = null;
    }
}
