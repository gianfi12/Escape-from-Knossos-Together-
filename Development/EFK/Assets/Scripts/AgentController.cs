using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{

    [SerializeField] private Transform target;
    private NavMeshAgent agent;

    [SerializeField] private float wanderRadius = 5;

    private bool isWandering = false;
    

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
        if (!isWandering)
        {
            isWandering = true;
            Seek(RandomNavMeshLocation());

            StartCoroutine(SetWandering(1.5f));
        }
    }

    IEnumerator SetWandering(float time)
    {
        yield return new WaitForSeconds(time);
        isWandering = false;
        yield return null;
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
        Wander();
    }
}
