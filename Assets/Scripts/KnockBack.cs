using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyKnockback : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float knockbackForceMultiplier = 5f;
    public float hitDuration = 0.5f; // sprite stays in hit state
    public GameObject spriteNormal;
    public GameObject spriteHit;

    private Rigidbody rb;
    private bool isInHitState = false;
    private Coroutine hitCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (spriteNormal != null) spriteNormal.SetActive(true);
        if (spriteHit != null) spriteHit.SetActive(false);
    }

    public void Knockback(Vector3 direction, float force)
    {
        // Apply physics knockback
        if (rb != null)
        {
            Vector3 knockDir = (direction + Vector3.up * 0.2f).normalized;
            rb.AddForce(knockDir * force * knockbackForceMultiplier, ForceMode.Impulse);
        }

        // Switch sprite
        SwitchToHitSprite();
    }

    private void SwitchToHitSprite()
    {
        if (isInHitState) return;

        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);

        if (spriteNormal != null) spriteNormal.SetActive(false);
        if (spriteHit != null) spriteHit.SetActive(true);

        isInHitState = true;
        hitCoroutine = StartCoroutine(SwitchBackAfterDelay());
    }

    private IEnumerator SwitchBackAfterDelay()
    {
        yield return new WaitForSeconds(hitDuration);
        if (spriteHit != null) spriteHit.SetActive(false);
        if (spriteNormal != null) spriteNormal.SetActive(true);
        isInHitState = false;
    }
}
