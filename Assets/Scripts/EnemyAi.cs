using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float wanderRadius = 5f;
    public float wanderInterval = 3f;

    private NavMeshAgent agent;
    private Coroutine wanderRoutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component missing on Enemy!");
        }
    }

    private void OnEnable()
    {
        if (wanderRoutine != null) StopCoroutine(wanderRoutine);
        wanderRoutine = StartCoroutine(WanderRoutine());
    }

    private void OnDisable()
    {
        if (wanderRoutine != null) StopCoroutine(wanderRoutine);
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            while (agent == null || !agent.enabled || !agent.isOnNavMesh)
            {
                yield return null;
            }

            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            yield return new WaitForSeconds(wanderInterval);
        }
    }

    public void WarpToPosition(Vector3 targetPosition)
    {
        if (agent == null) return;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 1f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
    }
}
