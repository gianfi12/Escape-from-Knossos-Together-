using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomCollider : MonoBehaviour
{
    [SerializeField] private RoomAbstract room;
    [SerializeField] private List<ActivatableObject> activatableObjects = new List<ActivatableObject>();
    [SerializeField] private AudioSource ambientSound;

    public AudioSource AmbientSound
    {
        get => ambientSound;
        set => ambientSound = value;
    }


    private bool _hasAlreadyBeenActivated = false;

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
                    if(!_hasAlreadyBeenActivated)
                    {
                        player.IncrementTimer(room.TimeIncrementInSeconds);
                        _hasAlreadyBeenActivated = true;
                    }

                    foreach(ActivatableObject o in activatableObjects) o.ActivateObject();
                    FindObjectOfType<AudioManager>().Stop("AmbientCorridor");
                    ambientSound.Play();
                }
            }
            else {
                player = other.GetComponent<PlayerControllerMap>();
                player.SetRoom(room);
                room.Player = player;
                // player.SetTimer(room.MaxTimeInSeconds, room.TimeoutTriggersLoss);
                if(!_hasAlreadyBeenActivated)
                {
                    player.IncrementTimer(room.TimeIncrementInSeconds);
                    _hasAlreadyBeenActivated = true;
                }
                foreach (ActivatableObject o in activatableObjects) o.ActivateObject();
                FindObjectOfType<AudioManager>().Stop("AmbientCorridor");
                ambientSound.Play();
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

        if (other.CompareTag("Player")) {
            PlayerControllerMap player;

            if (PhotonNetwork.IsConnected) {
                if (!other.GetComponent<PhotonView>().IsMine) {
                }
                else {
                    foreach (ActivatableObject o in activatableObjects) o.DeactivateObject();
                    ambientSound.Stop();
                    FindObjectOfType<AudioManager>().Play("AmbientCorridor");
                }
            }
            else {
                foreach (ActivatableObject o in activatableObjects) o.DeactivateObject();
                ambientSound.Stop();
                FindObjectOfType<AudioManager>().Play("AmbientCorridor");
            }
        }

    }
    

    public RoomAbstract Room
    {
        get => room;
        set => room = value;
    }
    
    

    public void AddActivatableObject(ActivatableObject o) {
        activatableObjects.Add(o);
    }
}
