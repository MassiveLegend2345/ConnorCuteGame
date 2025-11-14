using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
 
    public float wanderRadius = 10f;
    public float wanderInterval = 3f;
    public float wanderSpeed = 2f;

    public float fleeDuration = 2f;
    public float fleeSpeed = 6f;

    public bool knockedBack = false;

    public NavMeshAgent npc;
    private Transform player;

    private bool isFleeing = false;

    void Start()
    {
        if (npc == null) npc = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        npc.speed = wanderSpeed;
        StartCoroutine(WanderRoutine());
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            if (!isFleeing && !knockedBack)
            {
                Vector3 newPos = GetRandomWanderPosition();
                npc.SetDestination(newPos);
            }
            yield return new WaitForSeconds(wanderInterval);
        }
    }

    private Vector3 GetRandomWanderPosition()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius + transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDir, out hit, wanderRadius, NavMesh.AllAreas);
        return hit.position;
    }

    public void TriggerFlee()
    {
        if (isFleeing) return;
        StartCoroutine(FleeRoutine());
    }

    private IEnumerator FleeRoutine()
    {
        isFleeing = true;
        npc.speed = fleeSpeed;

        float timer = 0f;
        while (timer < fleeDuration)
        {
            if (!knockedBack)
            {
                Vector3 fleeDir = (transform.position - player.position).normalized;
                npc.SetDestination(transform.position + fleeDir * 12f);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        npc.speed = wanderSpeed;
        isFleeing = false;
    }
}
