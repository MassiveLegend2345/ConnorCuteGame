using UnityEngine;
using System.Collections;

public class EnemyKnockback : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float knockbackForceMultiplier = 5f;

    [Header("Sprite Objects")]
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

        // Set initial state
        if (spriteOne != null)
            spriteOne.SetActive(true);
        if (spriteTwo != null)
            spriteTwo.SetActive(false);
    }

    private void Update()
    {
        if (rb != null && !isInHitState)
        {
            // Detect sudden velocity changes (knockback)
            float velocityChange = (rb.linearVelocity - lastVelocity).magnitude;

            // If there's a sudden large velocity change, assume knockback occurred
            if (velocityChange > 10f) // Adjust this threshold as needed
            {
                Debug.Log($"Detected knockback! Velocity change: {velocityChange}");
                SwitchToHitSprite();
            }

            lastVelocity = rb.linearVelocity;
        }
    }

    public void Knockback(Vector3 direction, float force)
    {
        Debug.Log("Knockback method called directly");

        if (rb != null)
        {
            rb.AddForce(direction.normalized * force * knockbackForceMultiplier, ForceMode.Impulse);
        }

        SwitchToHitSprite();
    }

    private void SwitchToHitSprite()
    {
        if (isInHitState) return; // Already in hit state

        Debug.Log("SWITCHING TO SPRITE TWO");

        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }

        if (spriteOne != null)
        {
            spriteOne.SetActive(false);
            Debug.Log("Sprite One disabled");
        }

        if (spriteTwo != null)
        {
            spriteTwo.SetActive(true);
            Debug.Log("Sprite Two enabled");
        }

        isInHitState = true;
        hitCoroutine = StartCoroutine(SwitchBackAfterDelay());
    }

    private IEnumerator SwitchBackAfterDelay()
    {
        yield return new WaitForSeconds(hitDuration);

        Debug.Log("Switching back to Sprite One");

        if (spriteTwo != null)
            spriteTwo.SetActive(false);
        if (spriteOne != null)
            spriteOne.SetActive(true);

        isInHitState = false;
        hitCoroutine = null;
    }

    [ContextMenu("TEST SPRITE SWITCH")]
    public void TestSpriteSwitch()
    {
        Debug.Log("=== TESTING SPRITE SWITCH ===");
        SwitchToHitSprite();
    }
}