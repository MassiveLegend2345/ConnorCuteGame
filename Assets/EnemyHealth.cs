using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    public float respawnDelay = 2f;

    [Header("Invulnerability")]
    public float invulTime = 0.15f; // short invul to avoid duplicate triggers

    private int currentHealth;
    private Vector3 spawnPoint;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private Collider[] colliders;
    private Renderer[] renderers;
    private bool isDead = false;
    private bool isInvulnerable = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
        spawnPoint = transform.position;
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        isDead = false;
        isInvulnerable = false;
        EnableVisualsAndColliders(true);
    }

    public void TakeHit()
    {
        if (isDead)
        {
            Debug.Log($"[{name}] TakeHit ignored - already dead");
            return;
        }

        if (isInvulnerable)
        {
            Debug.Log($"[{name}] TakeHit ignored - invulnerable");
            return;
        }

        currentHealth--;
        Debug.Log($"[{name}] Took hit. Remaining HP: {currentHealth}");

        // short invulnerability so multiple colliders/punches in same frame won't double-hit
        if (invulTime > 0f)
            StartCoroutine(InvulCoroutine());

        if (currentHealth <= 0)
        {
            StartCoroutine(DieAndRespawn());
        }
    }

    private IEnumerator InvulCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulTime);
        isInvulnerable = false;
    }

    private IEnumerator DieAndRespawn()
    {
        isDead = true;
        Debug.Log($"[{name}] Died. Respawning in {respawnDelay} seconds.");

        // Disable visuals & nav/collider/physics but keep object active so scripts remain safe
        EnableVisualsAndColliders(false);

        // stop velocity
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Optionally disable NavMeshAgent so it doesn't try to path while "dead"
        if (agent != null)
            agent.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        // Respawn: reset position, enable agent, reset health
        transform.position = spawnPoint;

        if (agent != null)
        {
            agent.enabled = true;
            // Warp to spawn to avoid NavMeshAgent complaints
            agent.Warp(spawnPoint);
        }

        currentHealth = maxHealth;
        isDead = false;
        isInvulnerable = false;

        EnableVisualsAndColliders(true);
        Debug.Log($"[{name}] Respawned.");
    }

    private void EnableVisualsAndColliders(bool enable)
    {
        // toggle all colliders on children (including root)
        if (colliders != null)
        {
            foreach (var c in colliders)
            {
                // leave triggers (like hurtboxes) enabled if you want; this toggles all for simplicity
                c.enabled = enable;
            }
        }

        // toggle visual renderers
        if (renderers != null)
        {
            foreach (var r in renderers)
                r.enabled = enable;
        }
    }

    // helper to set spawn externally if you instantiate
    public void SetSpawnPoint(Vector3 pos)
    {
        spawnPoint = pos;
    }
}
