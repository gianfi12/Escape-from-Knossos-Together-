using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AgentController : MonoBehaviour
{

    private Transform target;
    private NavMeshAgent agent;
    private Animator animator;

    [SerializeField] private float wanderRadius = 5;
    private CheckpointManager checkpointManager;
    [SerializeField] private string checkpointName;
    [SerializeField] private bool isPatroller;
    private List<GameObject> checkpoints;
    
    private int currentCheckpoint = 0;

    private LineOfSight lineOfSight;
    private Material fovMaterial;
    [SerializeField] private Color standardFovColor;
    [SerializeField] private Color seekingFovColor;

    private bool isSeekingPlayer;
    private bool isWanderingAfterSeeking;

    private Vector3 currentMovement;
    private float lastDir;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (isPatroller)
        {
            checkpointManager = (CheckpointManager)FindObjectOfType(typeof(CheckpointManager));
            checkpoints = checkpointManager.getSelectedCheckpoint(checkpointName);
        
            if (checkpoints.Count != 0)
            {
                transform.position = checkpoints[0].transform.position;
            }
        }

        lineOfSight = GetComponent<LineOfSight>();
        fovMaterial = GetComponentInChildren<MeshRenderer>().material;
    }

    private void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    public void SeekPlayer(Vector3 location) {
        isSeekingPlayer = true;
        fovMaterial.SetColor("_Color", seekingFovColor);
        lineOfSight.viewAngle = 110;
        agent.speed = 3f;
        agent.SetDestination(location);
    }

    private void Wander()
    {
        Seek(RandomNavMeshLocation());
    }

    IEnumerator StopAgent(float delay) {
        agent.isStopped = true;
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
        if (agent.remainingDistance < 0.5 && agent.remainingDistance > 0.4) lastDir = SetDirection();
        if (isSeekingPlayer && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0) {
            isSeekingPlayer = false;
            isWanderingAfterSeeking = true;
            Wander();
        }
        else if (isWanderingAfterSeeking && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0) {
            StartCoroutine("StopAgent", 2.5f);
            isWanderingAfterSeeking = false;
        }
        else if (isPatroller && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0) {
            fovMaterial.SetColor("_Color", standardFovColor);
            lineOfSight.viewAngle = 50;
            agent.speed = 1.5f;
            Patrol();
        }

        if (!isPatroller && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
        {
            fovMaterial.SetColor("_Color", standardFovColor);
            lineOfSight.viewAngle = 50;
            agent.speed = 1.5f;
            Wander();
        }
        
        currentMovement = agent.velocity.normalized;
        animator.SetFloat("Horizontal", currentMovement.x);
        animator.SetFloat("Vertical", currentMovement.y);
        animator.SetFloat("Speed", currentMovement.sqrMagnitude);
        animator.SetFloat("Direction", lastDir);
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
}
