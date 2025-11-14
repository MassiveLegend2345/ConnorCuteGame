using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Game.NavigationTutorial;

public class EnemyAI : NPCComponent
{
    [Header("Wander Settings")]
    public float wanderRadius = 10f;
    public float wanderInterval = 3f;

    [Header("Flee Settings")]
    public float fleeDuration = 2f;
    public float fleeSpeed = 6f;
    public float wanderSpeed = 2f;

    // How far to search for the navmesh when snapping the agent
    public float navmeshSampleDistance = 2f;

    private Transform player;
    private FacePlayer facePlayerScript;
    private bool isFleeing = false;

    protected override void Awake()
    {
        base.Awake();
        facePlayerScript = GetComponentInChildren<FacePlayer>();
    }

    private void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (npc == null)
        {
            Debug.LogError($"[{name}] NPC component not found on parent.");
            enabled = false;
            return;
        }

        if (npc.Agent == null)
        {
            Debug.LogError($"[{name}] NavMeshAgent not found on NPC GameObject.");
            enabled = false;
            return;
        }

        // Attempt to ensure agent is placed on the navmesh now:
        if (!EnsureAgentIsOnNavMesh())
        {
            Debug.LogWarning($"[{name}] Agent is not on navmesh at Start. Will keep trying when needed.");
        }

        npc.Agent.speed = wanderSpeed;
        StartCoroutine(WanderRoutine());
    }

    // --------------------------------------------------------
    // WANDER LOGIC
    // --------------------------------------------------------
    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            if (!isFleeing)
            {
                // Only attempt to set destination when agent is ready
                if (EnsureAgentIsOnNavMesh())
                {
                    Vector3 newPos = GetRandomWanderPosition();
                    npc.Agent.SetDestination(newPos);
                }
                else
                {
                    // agent not ready - just log for debugging (only once per interval)
                    Debug.LogWarning($"[{name}] Wander skipped: agent not placed on NavMesh.");
                }
            }

            yield return new WaitForSeconds(wanderInterval);
        }
    }

    private Vector3 GetRandomWanderPosition()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        randomDir += transform.position;
        NavMeshHit hit;
        // Try to sample on the navmesh; fall back to current position if fails
        if (NavMesh.SamplePosition(randomDir, out hit, wanderRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }

    // --------------------------------------------------------
    // FLEE LOGIC
    // --------------------------------------------------------
    public void TriggerFlee()
    {
        if (isFleeing) return;
        StartCoroutine(FleeRoutine());
    }

    private IEnumerator FleeRoutine()
    {
        isFleeing = true;
        // make sure agent is on navmesh before we change speeds / destinations
        if (!EnsureAgentIsOnNavMesh())
        {
            Debug.LogWarning($"[{name}] Flee requested but agent not on navmesh. Attempting to enable anyway.");
        }

        npc.Agent.speed = fleeSpeed;

        if (facePlayerScript) facePlayerScript.facePlayer = false;

        float timer = 0f;
        while (timer < fleeDuration)
        {
            if (player == null) break;

            // compute flee target (away from player) and try to set as destination
            Vector3 fleeDir = (transform.position - player.position).normalized;
            Vector3 candidate = transform.position + fleeDir * 10f;

            // Sample the navmesh near the candidate, use fallback to current if not found
            NavMeshHit hit;
            Vector3 finalTarget = transform.position;
            if (NavMesh.SamplePosition(candidate, out hit, 5f, NavMesh.AllAreas))
            {
                finalTarget = hit.position;
            }
            else if (NavMesh.SamplePosition(transform.position, out hit, navmeshSampleDistance, NavMesh.AllAreas))
            {
                finalTarget = hit.position;
            }
            else
            {
                // no navmesh nearby; skip setting destination this frame
                Debug.LogWarning($"[{name}] Could not find NavMesh sample for flee target.");
                yield return null;
                timer += Time.deltaTime;
                continue;
            }

            if (npc.Agent != null && npc.Agent.enabled && npc.Agent.isOnNavMesh)
            {
                npc.Agent.SetDestination(finalTarget);
            }
            else
            {
                // try to place agent on navmesh and then set destination
                if (EnsureAgentIsOnNavMesh())
                {
                    npc.Agent.SetDestination(finalTarget);
                }
                else
                {
                    // if still not placeable, try once more next frame
                    Debug.LogWarning($"[{name}] Agent still not ready during flee; waiting.");
                }
            }

            // Make sprite visually face away from the player while fleeing
            if (facePlayerScript != null && player != null)
            {
                Vector3 look = transform.position - player.position;
                look.y = 0;
                facePlayerScript.transform.rotation = Quaternion.LookRotation(look);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // restore wander settings
        npc.Agent.speed = wanderSpeed;
        if (facePlayerScript) facePlayerScript.facePlayer = true;
        isFleeing = false;
    }

    // --------------------------------------------------------
    // NAVMESH SAFETY
    // --------------------------------------------------------
    /// <summary>
    /// Ensures the agent is enabled and placed on the NavMesh. If the agent is not on the navmesh,
    /// this will attempt to sample the navmesh near the transform.position and warp the agent there.
    /// Returns true if agent is present and placed on the navmesh.
    /// </summary>
    private bool EnsureAgentIsOnNavMesh()
    {
        var agent = npc.Agent;
        if (agent == null) return false;

        // if agent is already enabled and on navmesh, we're good
        if (agent.enabled && agent.isOnNavMesh) return true;

        // otherwise, try to sample navmesh near the transform
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, navmeshSampleDistance, NavMesh.AllAreas))
        {
            // place/warp agent to sample position
            agent.Warp(hit.position);
            agent.enabled = true;
            // double-check
            if (agent.isOnNavMesh) return true;
        }

        // failed
        return false;
    }
}
