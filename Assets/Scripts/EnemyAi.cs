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
        player = GameObject.FindGameObjectWithTag("Player").transform;
        npc.Agent.speed = wanderSpeed;
        StartCoroutine(WanderRoutine());
    }

    // --------------------------------------------------------------------

    #region WANDER LOGIC
    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            if (!isFleeing)
            {
                Vector3 newPos = GetRandomWanderPosition();
                npc.Agent.SetDestination(newPos);
            }

            yield return new WaitForSeconds(wanderInterval);
        }
    }

    private Vector3 GetRandomWanderPosition()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        randomDir += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDir, out hit, wanderRadius, NavMesh.AllAreas);
        return hit.position;
    }
    #endregion

    // --------------------------------------------------------------------

    #region FLEE LOGIC

    public void TriggerFlee()
    {
        if (isFleeing) return;
        StartCoroutine(FleeRoutine());
    }

    private IEnumerator FleeRoutine()
    {
        isFleeing = true;

        npc.Agent.speed = fleeSpeed;

        // Turn off facing toward player -> make sprite show “back”
        if (facePlayerScript) facePlayerScript.facePlayer = false;

        float timer = 0f;

        while (timer < fleeDuration)
        {
            Vector3 fleeDir = (transform.position - player.position).normalized;
            Vector3 fleeTarget = transform.position + fleeDir * 10f;

            npc.Agent.SetDestination(fleeTarget);

            // MAKE SPRITE FACE AWAY FROM PLAYER
            if (facePlayerScript != null)
            {
                Vector3 look = transform.position - player.position;
                look.y = 0;
                facePlayerScript.transform.rotation = Quaternion.LookRotation(look);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Return to wandering
        npc.Agent.speed = wanderSpeed;
        if (facePlayerScript) facePlayerScript.facePlayer = true;

        isFleeing = false;
    }

    #endregion
}
