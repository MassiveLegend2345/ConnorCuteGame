using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyKnockback : MonoBehaviour
{
    public float knockbackForceMultiplier = 5f;

    public GameObject spriteOne;
    public GameObject spriteTwo;
    public float hitDuration = 5f;

    private Rigidbody rb;
    private Coroutine hitCoroutine;
    private Vector3 lastVelocity;
    private bool isInHitState = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (spriteOne != null)
            spriteOne.SetActive(true);
        if (spriteTwo != null)
            spriteTwo.SetActive(false);
    }

    private void Update()
    {
        if (rb != null && !isInHitState)
        {
            float velocityChange = (rb.linearVelocity - lastVelocity).magnitude;
            if (velocityChange > 10f)
            {
                Debug.Log($"SENDDDD: {velocityChange}");
                SwitchToHitSprite();
            }

            lastVelocity = rb.linearVelocity;
        }
    }

    public void Knockback(Vector3 direction, float force)
    {
        var agent = GetComponentInParent<EnemyAI>()?.npc.Agent;

        if (rb != null)
        {
            // Temporarily disable NavMeshAgent so physics can affect the enemy
            if (agent != null) agent.enabled = false;

            // Add upward component to ensure height
            Vector3 knockDir = (direction + Vector3.up * 0.5f).normalized;
            rb.AddForce(knockDir * force * knockbackForceMultiplier, ForceMode.Impulse);

            // Re-enable agent after a short delay
            StartCoroutine(ReenableAgent(agent, 0.5f));
        }

        // Trigger flee behavior from EnemyAI
        GetComponentInParent<EnemyAI>()?.TriggerFlee();

        SwitchToHitSprite();
    }

    private IEnumerator ReenableAgent(NavMeshAgent agent, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (agent != null) agent.enabled = true;
    }

    private void SwitchToHitSprite()
    {
        if (isInHitState) return;

        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }

        if (spriteOne != null)
            spriteOne.SetActive(false);

        if (spriteTwo != null)
            spriteTwo.SetActive(true);

        isInHitState = true;
        hitCoroutine = StartCoroutine(SwitchBackAfterDelay());
    }

    private IEnumerator SwitchBackAfterDelay()
    {
        yield return new WaitForSeconds(hitDuration);

        if (spriteTwo != null)
            spriteTwo.SetActive(false);
        if (spriteOne != null)
            spriteOne.SetActive(true);

        isInHitState = false;
        hitCoroutine = null;
    }

    [ContextMenu("Test")]
    public void TestSpriteSwitch()
    {
        SwitchToHitSprite();
    }
}
