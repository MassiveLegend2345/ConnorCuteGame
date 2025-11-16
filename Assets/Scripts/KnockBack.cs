using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyKnockback : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float knockbackForceMultiplier = 5f;

    [Header("Sprites")]
    public GameObject spriteOne;
    public GameObject spriteTwo;
    public GameObject explosionSprite; // Drag your explosion sprite here
    public float hitDuration = 0.5f;

    [Header("Audio")]
    public AudioClip[] hitSounds;
    public AudioClip[] punchSounds;
    public AudioClip explosionSound; // Drag your explosion SFX here
    public AudioSource audioSource;

    private int currentHitSound = 0;
    private int currentPunchSound = 0;
    private Rigidbody rb;
    private Coroutine hitCoroutine;
    private bool isInHitState = false;
    private EnemyHealth enemyHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemyHealth = GetComponent<EnemyHealth>();

        // Set initial sprite - explosion sprite starts disabled
        if (spriteOne != null) spriteOne.SetActive(true);
        if (spriteTwo != null) spriteTwo.SetActive(false);
        if (explosionSprite != null) explosionSprite.SetActive(false);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void RefreshRigidbody()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }

    public void Knockback(Vector3 direction, float force, bool isPunch = false)
    {
        if (enemyHealth != null && enemyHealth.IsDead())
            return;

        if (rb == null) RefreshRigidbody();
        if (rb != null && !rb.isKinematic)
        {
            Vector3 knockDir = (direction + Vector3.up * 0.5f).normalized;
            if (float.IsNaN(knockDir.x) || float.IsNaN(knockDir.y) || float.IsNaN(knockDir.z))
                knockDir = Vector3.forward;

            rb.AddForce(knockDir * force * knockbackForceMultiplier, ForceMode.Impulse);
        }

        SwitchToHitSprite();

        if (isPunch) PlayPunchSound();
        PlayHitSound();
    }

    // NEW: Show explosion sprite and play SFX when enemy dies
    public void TriggerExplosion()
    {
        // Switch to explosion sprite
        if (spriteOne != null) spriteOne.SetActive(false);
        if (spriteTwo != null) spriteTwo.SetActive(false);
        if (explosionSprite != null) explosionSprite.SetActive(true);

        // Play explosion sound
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }
    }

    private void PlayHitSound()
    {
        if (hitSounds.Length == 0 || audioSource == null) return;
        audioSource.PlayOneShot(hitSounds[currentHitSound]);
        currentHitSound = (currentHitSound + 1) % hitSounds.Length;
    }

    private void PlayPunchSound()
    {
        if (punchSounds.Length == 0 || audioSource == null) return;
        audioSource.PlayOneShot(punchSounds[currentPunchSound]);
        currentPunchSound = (currentPunchSound + 1) % punchSounds.Length;
    }

    private void SwitchToHitSprite()
    {
        if (isInHitState) return;
        if (hitCoroutine != null) StopCoroutine(hitCoroutine);

        if (spriteOne != null) spriteOne.SetActive(false);
        if (spriteTwo != null) spriteTwo.SetActive(true);

        isInHitState = true;
        hitCoroutine = StartCoroutine(SwitchBackAfterDelay());
    }

    private IEnumerator SwitchBackAfterDelay()
    {
        yield return new WaitForSeconds(hitDuration);

        if (enemyHealth != null && enemyHealth.IsDead())
            yield break;

        if (spriteTwo != null) spriteTwo.SetActive(false);
        if (spriteOne != null) spriteOne.SetActive(true);

        isInHitState = false;
        hitCoroutine = null;
    }
}