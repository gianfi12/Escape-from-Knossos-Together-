using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AgentController : ActivatableObject
{

    private Transform target;
    private NavMeshAgent agent;
    private Animator animator;

    [SerializeField] private float wanderRadius = 5;
    [SerializeField] private bool isPatroller;
    [SerializeField] private List<GameObject> checkpoints;
   
    private int currentCheckpoint = 0;
    
    [SerializeField] private GameObject eyeObject;
    private SpriteRenderer eyeSpriteRenderer;
    private LineOfSight lineOfSight;
    private Material fovMaterial;

    [SerializeField] private Color standardFovColor;
    [SerializeField] private Color seekingFovColor;
    [SerializeField] private Color standardEyeColor;
    [SerializeField] private Color seekingEyeColor;
    [SerializeField] private AudioSource triggerOn;
    [SerializeField] private AudioSource triggerOff;
    [SerializeField] private AudioSource triggerRhytm;


    private bool isSeekingPlayer;
    private bool isWanderingAfterSeeking;

    private Vector3 currentMovement;
    private float lastDir;
    private Vector3 _previousPosition;
    private bool isSoundStart;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // HARDCODED FIX FOR AGENTS SOMETIMES GETTING ROTATED WHEN PLAYING ONLINE.
        // find a better solution if possible
        if (transform.rotation.x != 0) transform.rotation = Quaternion.identity;
        _previousPosition = transform.position;
        

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (isPatroller)
        {
            if (checkpoints.Count != 0)
            {
                transform.position = checkpoints[0].transform.position;
            }
        }

        lineOfSight = GetComponent<LineOfSight>();
        fovMaterial = GetComponentInChildren<MeshRenderer>().material;

        eyeSpriteRenderer = eyeObject.GetComponent<SpriteRenderer>();

        if(!isPatroller) StartCoroutine("ResetTargetWithDelay", 1f);
    }

    private void Start()
    {
        Random.InitState(transform.GetComponentInParent<ObjectsContainer>().Seed);
        DeactivateObject();
    }

    private void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    public void SeekPlayer(Vector3 location) {
        isSeekingPlayer = true;
        fovMaterial.SetColor("_Color", seekingFovColor);
        lineOfSight.viewAngle = 110;
        eyeSpriteRenderer.color = seekingEyeColor;
        //eyeSpriteRenderer.material.SetColor("GlowColor", seekingEyeColor*16);
        agent.speed = 2.5f;
        agent.SetDestination(location);
        if (!isSoundStart)
        {
            triggerOn.Play();
            triggerRhytm.PlayDelayed(0.5f);
            isSoundStart = true;
        }
    }

    private void Wander()
    {
        Seek(RandomNavMeshLocation());
    }

    IEnumerator StopAgent(float delay) {
        agent.isStopped = true;
        isSoundStart = false;
        triggerRhytm.Stop();
        triggerOff.Play();
        GetComponentInChildren<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(delay);
        GetComponentInChildren<MeshRenderer>().enabled = true;
        agent.isStopped = false;
    }

    private void Patrol()
    {
        if (checkpoints.Count == 0) return;
        Seek(checkpoints[currentCheckpoint].transform.position);
        currentCheckpoint = (currentCheckpoint + 1) % checkpoints.Count;
    }
    
    private Vector3 RandomNavMeshLocation()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (isSeekingPlayer && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0) {
            isSeekingPlayer = false;
            isWanderingAfterSeeking = true;
            Wander();
        }
        else if (isWanderingAfterSeeking && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0) {
            StartCoroutine("StopAgent", 2.5f);
            isWanderingAfterSeeking = false;
        }
        else if (isPatroller && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance ==0) {
            fovMaterial.SetColor("_Color", standardFovColor);
            lineOfSight.viewAngle = 50;
            eyeSpriteRenderer.color = standardEyeColor;
           // eyeSpriteRenderer.material.SetColor("GlowColor", seekingEyeColor * 16);
            agent.speed = 1.5f;
            Patrol();
        }

        if (!isPatroller && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
        {
            fovMaterial.SetColor("_Color", standardFovColor);
            lineOfSight.viewAngle = 50;
            eyeSpriteRenderer.color = standardEyeColor;
            //eyeSpriteRenderer.material.SetColor("GlowColor", seekingEyeColor * 16);
            agent.speed = 1.5f;
            Wander();  
        }

        currentMovement = agent.velocity.normalized;
        animator.SetFloat("Horizontal", currentMovement.x);
        //animator.SetFloat("Vertical", currentMovement.y);
        //animator.SetFloat("Speed", currentMovement.sqrMagnitude);
        //animator.SetFloat("Direction", lastDir);  
    }

    public float GetDirectionAngle() {
        return Vector3.SignedAngle(transform.up, agent.velocity.normalized, Vector3.forward);
    }
    
    private float SetDirection()
    {
        if (Mathf.Abs(currentMovement.x) > Double.Epsilon && Math.Abs(currentMovement.x) > Mathf.Abs(currentMovement.y))
        {
            if (currentMovement.x > Double.Epsilon) return 3; //right
            return 4; //left
        }
        else
        {
            if (currentMovement.y > Double.Epsilon) return 2; //up
            return 1; //down
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isSeekingPlayer || isWanderingAfterSeeking)
        {
            PlayerControllerMap playerControllerMap;
            if (other.CompareTag("PlayerFeet"))
            { 
                playerControllerMap = other.GetComponentInParent<PlayerControllerMap>();
            }
            else if (other.CompareTag("Player"))
            { 

                playerControllerMap = other.GetComponent<PlayerControllerMap>();
            }
            else return;

            if (!playerControllerMap.gameObject.GetComponent<PlayerInput>()._isHidden)
            {
                if (PhotonNetwork.IsConnected)
                {
                    if (playerControllerMap.GetComponent<PhotonView>().IsMine)
                    {
                        playerControllerMap.SetPlayerIsDead();
                    }
                }
                else
                {
                    playerControllerMap.SetPlayerIsDead();
                    playerControllerMap.SetPlayerIsDead();
                }
            }
        }
    }

    public void SetCheckpoints(List<GameObject> checkpoints)
    {
        this.checkpoints = new List<GameObject>(checkpoints);
    }

    public bool IsPatroller
    {
        get => isPatroller;
        set => isPatroller = value;
    }

    IEnumerator ResetTargetWithDelay(float delay) {
        while (true) {
            yield return new WaitForSeconds(delay);
            ResetTarget();
        }
    }

    private void ResetTarget() {
        if (Math.Abs(Vector3.Distance(transform.position, _previousPosition)) < 0.5) {
            Wander();
        }

        _previousPosition = transform.position;
    }

    public override void ActivateObject()
    {
        agent.isStopped = false;
        lineOfSight.enabled = true;
        GetComponentInChildren<MeshRenderer>().enabled = true;
        lineOfSight.NpcStartFindTarget();
    }

    public override void DeactivateObject()
    {
        agent.isStopped = true;
        agent.SetDestination(gameObject.transform.position);
        isSeekingPlayer = false;
        isWanderingAfterSeeking = false;
        lineOfSight.enabled = false;
        GetComponentInChildren<MeshRenderer>().enabled = false;
        if (triggerRhytm.isPlaying)
        {
            triggerRhytm.Stop();
            triggerOff.Play();
        }
    }
}
