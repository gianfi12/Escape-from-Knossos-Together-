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

    private List<GameObject> players = new List<GameObject>();

    private bool movingPlayers;
    private bool[] checkpointReached;
    private bool finalBoss;
    private bool[] finalCheckpointReached;
    private bool setUpFinal;
    private bool isBossExploded;

    private void Start() {
        if (PhotonNetwork.IsConnected) playersNumber = 2;
        else playersNumber = 1;

        checkpointReached = new bool[playersNumber];
        finalCheckpointReached = new bool[playersNumber];
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            playersArrived++;
            players.Add(other.gameObject);

            foreach (GameObject player in players) player.GetComponent<PlayerControllerMap>().SetTimer(0, false);
        }

        if (playersArrived >= playersNumber)
        {
            StartCoroutine(controlledDoor.OpenDoorsWithDelay(0.5f));
            foreach (GameObject player in players)
            {
                PlayerInput playerInput = player.GetComponent<PlayerInput>();
                playerInput._canMove = false;
                playerInput._isFinal = true;
                playerInput.Movement = new Vector2(0,0);
            }
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

                /*if (!prevCheckpointReached) {
                    endgameVCAM.gameObject.SetActive(true); // activate VCAM only the first time
                    FindObjectOfType<AudioManager>().Play("WinTheme"); // play sound only the first time
                }*/

                if (!finalBoss)
                {
                    direction2D = Vector2.up;
                    foreach (GameObject player in players) {
                        if (player != null)
                        {
                            if (PhotonNetwork.IsConnected)
                            {
                                if (player.GetComponent<PhotonView>().IsMine)
                                {
                                    player.GetComponent<PlayerControllerMap>().Move(Vector3.up);
                                    animator = player.GetComponent<Animator>();
                                    animator.SetFloat("Speed", direction2D.SqrMagnitude());
                                    animator.SetFloat("Horizontal", direction2D.x);
                                }
                                
                            }
                            else
                            {
                                player.GetComponent<PlayerControllerMap>().Move(Vector3.up);
                                animator = player.GetComponent<Animator>();
                                animator.SetFloat("Speed", direction2D.SqrMagnitude());
                                animator.SetFloat("Horizontal", direction2D.x);
                            }
                            if (player.transform.position.y >= finalPosition.position.y) finalBoss = true;
                        }
                    }
                }
                else
                {
                    if (!finalCheckpointReached.All(x => x))
                    {
                        for (int i = 0; i < playersNumber; i++) {
                            if (players[i] != null)
                            {
                                if (Vector3.Distance(players[i].transform.position, finalCheckpoints[i].transform.position) > 0.1f && !finalCheckpointReached[i]) {
                                    Vector3 directionToCheckpoint = (finalCheckpoints[i].transform.position - players[i].transform.position).normalized;

                                    direction2D = new Vector2(directionToCheckpoint.x, directionToCheckpoint.y);
                                    players[i].GetComponent<PlayerControllerMap>().Move(directionToCheckpoint);

                                    animator = players[i].GetComponent<Animator>();
                                    animator.SetFloat("Speed", direction2D.SqrMagnitude());
                                    animator.SetFloat("Horizontal", direction2D.x);
                                }
                                else
                                {
                                    finalCheckpointReached[i] = true;
                                    animator = players[i].GetComponent<Animator>();
                                    animator.SetFloat("Speed", 0);
                                    animator.SetFloat("Horizontal",  (boss.transform.position - players[i].transform.position).x);
                                }    
                            }
                        }
                    }
                    
                    if (!setUpFinal)
                    {
                        boss.ActivateAnimator(this);
                        
                        foreach (GameObject player in players) {
                            if (player != null)
                            {
                                //animator = player.GetComponent<Animator>();
                                //animator.SetFloat("Speed", 0);
                                player.GetComponent<PlayerControllerMap>().SetTimer(999,true);
                            }
                        }
                        endgameVCAM.gameObject.SetActive(true);
                        setUpFinal = true;
                    }

                    if (isBossExploded)
                    {
                        direction2D = Vector2.up;
                        foreach (GameObject player in players) {
                            if (player != null)
                            {
                                if (PhotonNetwork.IsConnected)
                                {
                                    if (player.GetComponent<PhotonView>().IsMine)
                                    {
                                        player.GetComponent<PlayerControllerMap>().SetTimer(0,false);
                                        player.GetComponent<PlayerControllerMap>().Move(Vector3.up);
                                        animator = player.GetComponent<Animator>();
                                        animator.SetFloat("Speed", direction2D.SqrMagnitude());
                                        animator.SetFloat("Horizontal", direction2D.x);  
                                    }
                                }
                                else
                                {
                                    player.GetComponent<PlayerControllerMap>().SetTimer(0,false);
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
}
