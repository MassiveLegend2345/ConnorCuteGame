using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyKnockback : MonoBehaviour
{
    public float knockbackForceMultiplier = 5f;

    public GameObject spriteOne;
    public GameObject spriteTwo;
    public float hitDuration = 0.5f;

    [Header("Hit Sounds")]
    public AudioClip[] hitSounds;        // Assign 4 clips in Inspector
    public AudioSource audioSource;      // Assign the AudioSource on the enemy
    private int currentSoundIndex = 0;

    private Rigidbody rb;
    private Coroutine hitCoroutine;
    private bool isInHitState = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (spriteOne != null) spriteOne.SetActive(true);
        if (spriteTwo != null) spriteTwo.SetActive(false);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Knockback(Vector3 direction, float force)
    {
        // Play hit sound
        PlayHitSound();

        // Knockback physics
        if (rb != null)
        {
            Vector3 knockDir = (direction + Vector3.up * 0.5f).normalized;
            rb.AddForce(knockDir * force * knockbackForceMultiplier, ForceMode.Impulse);
        }

        // Switch sprite
        SwitchToHitSprite();
    }

    private void PlayHitSound()
    {
        if (hitSounds.Length == 0 || audioSource == null) return;

        audioSource.clip = hitSounds[currentSoundIndex];
        audioSource.Play();

        currentSoundIndex++;
        if (currentSoundIndex >= hitSounds.Length)
            currentSoundIndex = 0; // loop
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

        if (spriteTwo != null) spriteTwo.SetActive(false);
        if (spriteOne != null) spriteOne.SetActive(true);

        isInHitState = false;
        hitCoroutine = null;
    }
}
