using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private int playersArrived = 0;
    private int playersNumber;

    [SerializeField] private Doors controlledDoor;

    private void Start() {
        if (PhotonNetwork.IsConnected) playersNumber = 2;
        else playersNumber = 1;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) playersArrived++;

        if (playersArrived >= playersNumber) {
            controlledDoor.OpenDoors();
        }
    }
}
