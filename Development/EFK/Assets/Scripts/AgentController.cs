using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{

    [SerializeField] private Transform target;
    private NavMeshAgent agent;

    [SerializeField] private float wanderRadius = 5;
    [SerializeField] private Transform[] checkpoints;
    private int currentCheckpoint = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Seek(Vector3 location)
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
            if (checkpoints.Length == 0) return;
        
            Seek(checkpoints[currentCheckpoint].position);
            currentCheckpoint = (currentCheckpoint + 1) % checkpoints.Length;
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
        Patrol();
    }
}
