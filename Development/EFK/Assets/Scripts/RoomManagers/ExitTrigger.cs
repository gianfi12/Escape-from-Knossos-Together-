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
    [SerializeField] private Transform finalPosition;
    [SerializeField] private List<Transform> finalCheckpoints;
    [SerializeField] private Boss boss;

    private GameObject myPlayer;
    private List<GameObject> players = new List<GameObject>();

    private bool movingPlayer;
    private bool[] checkpointReached;
    private bool finalBoss;
    private bool[] finalCheckpointReached;
    private bool setUpFinal;
    private bool isBossExploded;

    private void Start() {
        if (PhotonNetwork.IsConnected) playersNumber = 2;
        else playersNumber = 1;
        
        finalCheckpointReached = new bool[playersNumber];
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playersArrived++;
            if (PhotonNetwork.IsConnected) {
                if (other.GetComponent<PhotonView>().IsMine) {
                    myPlayer = other.gameObject;
                    myPlayer.GetComponent<PlayerControllerMap>().SetTimer(0, false);
                }
            }
            else {
                myPlayer = other.gameObject;
                myPlayer.GetComponent<PlayerControllerMap>().SetTimer(0, false);
            }
            players.Add(other.gameObject);
        }

        if (playersArrived >= playersNumber)
        {
            StartCoroutine(controlledDoor.OpenDoorsWithDelay(0.5f));

            PlayerInput playerInput = myPlayer.GetComponent<PlayerInput>();
            playerInput._canMove = false;
            playerInput._isFinal = true;
            playerInput.Movement = new Vector2(0, 0);
            checkpointReached = new bool[playersNumber];
            movingPlayer = true;
        }
    }

    private void FixedUpdate() {
        if (movingPlayer) {
            Vector2 direction2D;
            Animator animator;

            for (int i = 0; i < playersNumber; i++) {
                if (players[i] != null) {
                    if (!checkpointReached[i]) {
                        if (players[i] == myPlayer) {
                            Vector3 directionToCheckpoint = (positionCheckpoints[i].transform.position - players[i].transform.position).normalized;

                            direction2D = new Vector2(directionToCheckpoint.x, directionToCheckpoint.y);
                            players[i].GetComponent<PlayerControllerMap>().Move(directionToCheckpoint);

                            animator = players[i].GetComponent<Animator>();
                            animator.SetFloat("Speed", direction2D.SqrMagnitude());
                            animator.SetFloat("Horizontal", direction2D.x);
                        }
                    }
                }
                else checkpointReached[i] = true;
            }

            if (checkpointReached.All(x => x)) {
                if (!finalBoss)
                {
                    direction2D = Vector2.up;

                    myPlayer.GetComponent<PlayerControllerMap>().Move(Vector3.up);
                    Debug.Log(myPlayer);
                    Debug.Log(myPlayer.GetComponent<PhotonView>().ViewID);
                    animator = myPlayer.GetComponent<Animator>();
                    animator.SetFloat("Speed", direction2D.SqrMagnitude());
                    animator.SetFloat("Horizontal", direction2D.x);
                    if (myPlayer.transform.position.y >= finalPosition.position.y) finalBoss = true;
                }
                else
                {
                    if (!finalCheckpointReached.All(x => x))
                    {
                        for (int i = 0; i < playersNumber; i++) {
                            if (players[i] != null)
                            {
                                if (Vector3.Distance(players[i].transform.position, finalCheckpoints[i].transform.position) > 0.1f && !finalCheckpointReached[i]) {
                                    if (players[i] == myPlayer) {
                                        Vector3 directionToCheckpoint = (finalCheckpoints[i].transform.position - players[i].transform.position).normalized;

                                        direction2D = new Vector2(directionToCheckpoint.x, directionToCheckpoint.y);
                                        players[i].GetComponent<PlayerControllerMap>().Move(directionToCheckpoint);

                                        animator = players[i].GetComponent<Animator>();
                                        animator.SetFloat("Speed", direction2D.SqrMagnitude());
                                        animator.SetFloat("Horizontal", direction2D.x);
                                    }
                                }
                                else
                                {
                                    finalCheckpointReached[i] = true;
                                    if (players[i] == myPlayer) {
                                        animator = players[i].GetComponent<Animator>();
                                        animator.SetFloat("Speed", 0);
                                        animator.SetFloat("Horizontal", (boss.transform.position - players[i].transform.position).x);
                                    }
                                }    
                            }
                        }
                    }
                    
                    if (!setUpFinal)
                    {
                        boss.ActivateAnimator(this);
                        myPlayer.GetComponent<PlayerControllerMap>().SetTimer(999, true);
                        endgameVCAM.gameObject.SetActive(true);
                        setUpFinal = true;
                    }

                    if (isBossExploded)
                    {
                        direction2D = Vector2.up;
                        myPlayer.GetComponent<PlayerControllerMap>().SetTimer(0, false);
                        myPlayer.GetComponent<PlayerControllerMap>().Move(Vector3.up);
                        animator = myPlayer.GetComponent<Animator>();
                        animator.SetFloat("Speed", direction2D.SqrMagnitude());
                        animator.SetFloat("Horizontal", direction2D.x);
                    }
                }
                
            }
        }
    }

    public void SetBossExploded()
    {
        StartCoroutine(SetTrigger());
    }
    
    IEnumerator SetTrigger()
    {
        yield return new WaitForSeconds(3);
        isBossExploded = true;
    }

    public void CheckpointReached(Checkpoint checkpoint, GameObject player) {
        int i = positionCheckpoints.IndexOf(checkpoint.gameObject.transform);
        if (checkpointReached != null && players.IndexOf(player) == i) checkpointReached[i] = true;
    }
}
