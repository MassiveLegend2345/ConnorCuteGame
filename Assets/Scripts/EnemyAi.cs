using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Wander Settings")]
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
            // Wait until agent is enabled and on NavMesh
            while (agent == null || !agent.enabled || !agent.isOnNavMesh)
            {
                yield return null;
            }

            // Pick a random point within wander radius
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            {
                // Wait a frame after warp to avoid SetDestination errors
                agent.SetDestination(hit.position);
            }

            yield return new WaitForSeconds(wanderInterval);
        }
    }

    /// <summary>
    /// Force agent to a position safely (for respawn)
    /// </summary>
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
