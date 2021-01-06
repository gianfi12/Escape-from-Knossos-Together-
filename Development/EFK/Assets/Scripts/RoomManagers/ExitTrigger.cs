using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExitTrigger : MonoBehaviour {
    private int playersArrived = 0;
    private int playersNumber;

    [SerializeField] private Doors controlledDoor;
    [SerializeField] private List<Transform> positionCheckpoints;
    [SerializeField] private CinemachineVirtualCamera endgameVCAM;

    private List<GameObject> players = new List<GameObject>();

    private bool movingPlayers;
    private bool[] checkpointReached;

    private void Start() {
        if (PhotonNetwork.IsConnected) playersNumber = 2;
        else playersNumber = 1;

        checkpointReached = new bool[playersNumber];
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playersArrived++;
            players.Add(other.gameObject);

            foreach (GameObject player in players) player.GetComponent<PlayerControllerMap>().SetTimer(0, false);
        }

        if (playersArrived >= playersNumber) {
            controlledDoor.OpenDoors();
            foreach(GameObject player in players) player.GetComponent<PlayerInput>().enabled = false;
            movingPlayers = true;
        }
    }

    private void Update() {
        if (movingPlayers) {
            bool prevCheckpointReached = checkpointReached.All(x => x);
            Vector2 direction2D;
            Animator animator;

            for (int i = 0; i < playersNumber; i++) {
                if (players[i] != null)
                {
                    if (Vector3.Distance(players[i].transform.position, positionCheckpoints[i].transform.position) > 0.1f && !checkpointReached[i]) {
                        Vector3 directionToCheckpoint = (positionCheckpoints[i].transform.position - players[i].transform.position).normalized;

                        direction2D = new Vector2(directionToCheckpoint.x, directionToCheckpoint.y);
                        players[i].GetComponent<PlayerControllerMap>().Move(directionToCheckpoint);

                        animator = players[i].GetComponent<Animator>();
                        animator.SetFloat("Speed", direction2D.SqrMagnitude());
                        animator.SetFloat("Horizontal", direction2D.x);
                    }
                    else checkpointReached[i] = true;    
                }
            }

            if (checkpointReached.All(x => x)) {
                if(!prevCheckpointReached) endgameVCAM.gameObject.SetActive(true); // activate VCAM only the first time

                direction2D = Vector2.up;
                foreach (GameObject player in players) {
                    if (player != null)
                    {
                        player.GetComponent<PlayerControllerMap>().Move(Vector3.up);
                        animator = player.GetComponent<Animator>();
                        animator.SetFloat("Speed", direction2D.SqrMagnitude());
                        animator.SetFloat("Horizontal", direction2D.x);
                    }
                }
            }
        }
    }
}
