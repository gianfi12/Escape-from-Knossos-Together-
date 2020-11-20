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
    [SerializeField] private bool isWanderer;
    private List<GameObject> checkpoints;
    
    private int currentCheckpoint = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        checkpointManager = (CheckpointManager)FindObjectOfType(typeof(CheckpointManager));
        checkpoints = checkpointManager.getSelectedCheckpoint(checkpointName);
        
        if (checkpoints.Count != 0)
        {
            transform.position = checkpoints[0].transform.position;
        }
    }

    public void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    private void Wander()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Seek(RandomNavMeshLocation());
        }
    }

    private void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (checkpoints.Count == 0) return;
        
            Seek(checkpoints[currentCheckpoint].transform.position);
            currentCheckpoint = (currentCheckpoint + 1) % checkpoints.Count;
        }
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
        if (isWanderer) Wander();
        else Patrol();

        Vector3 currentMovement = agent.velocity.normalized;
        animator.SetFloat("Horizontal", currentMovement.x);
        animator.SetFloat("Vertical", currentMovement.y);
        animator.SetFloat("Speed", currentMovement.sqrMagnitude);
        //animator.SetFloat("Direction", currentMovement.x);
    }

    public float GetDirectionAngle() {
        return Vector3.SignedAngle(transform.up, agent.velocity.normalized, Vector3.forward);
    }
}
