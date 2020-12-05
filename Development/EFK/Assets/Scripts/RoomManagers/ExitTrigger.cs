using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTrigger : MonoBehaviour {
    private int playersArrived = 0;
    private int playersNumber;

    [SerializeField] private Doors controlledDoor;
    [SerializeField] private List<Transform> positionCheckpoints;

    private List<GameObject> players = new List<GameObject>();

    private bool movingPlayers;

    private void Start() {
        if (PhotonNetwork.IsConnected) playersNumber = 2;
        else playersNumber = 1;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playersArrived++;
            players.Add(other.gameObject);
        }

        if (playersArrived >= playersNumber) {
            controlledDoor.OpenDoors();
            movingPlayers = true;
        }
    }

    private void Update() {
        if (movingPlayers) {
            for (int i = 0; i < playersNumber; i++) {
                Vector3 directionToCheckpoint = (positionCheckpoints[i].transform.position - players[i].transform.position).normalized;
                Vector2 direction2D = new Vector2(directionToCheckpoint.x, directionToCheckpoint.y);

                if (Vector3.Distance(players[i].transform.position, positionCheckpoints[i].transform.position) > 0.1f) {
                    players[i].GetComponent<PlayerInput>().enabled = false;
                    players[i].GetComponent<PlayerControllerMap>().Move(directionToCheckpoint);
                }
                else direction2D = Vector2.zero;

                Animator animator = players[i].GetComponent<Animator>();
                animator.SetFloat("Speed", direction2D.SqrMagnitude());
                animator.SetFloat("Horizontal", direction2D.x);
            }
        }  
    }
}
