using UnityEngine;
using System.Collections;

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
        if (rb != null)
        {
            rb.AddForce(direction.normalized * force * knockbackForceMultiplier, ForceMode.Impulse);
        }

        SwitchToHitSprite();
    }

    private void SwitchToHitSprite()
    {
        if (isInHitState) return; 

        Debug.Log("Workeddd bitch");

        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }

        if (spriteOne != null)
        {
            spriteOne.SetActive(false);
            Debug.Log("Sprite1");
        }

        if (spriteTwo != null)
        {
            spriteTwo.SetActive(true);
            Debug.Log("Sprite2");
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

    [ContextMenu("Test")]
    public void TestSpriteSwitch()
    {
        Debug.Log("Connor you did it you're a real hero you really fuccking did it holy shit");
        SwitchToHitSprite();
    }
}