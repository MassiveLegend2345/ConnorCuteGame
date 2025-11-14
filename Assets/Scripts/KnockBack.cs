using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyKnockback : MonoBehaviour
{
    public float knockbackForceMultiplier = 5f;

    public GameObject spriteOne;
    public GameObject spriteTwo;
    public float hitDuration = 0.5f;

    public AudioClip[] hitSounds;       
    public AudioClip[] punchSounds;      
    public AudioSource audioSource;      
    private int currentHitSound = 0;
    private int currentPunchSound = 0;

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

    public void Knockback(Vector3 direction, float force, bool isPunch = false)
    {
        if (rb != null)
        {
            Vector3 knockDir = (direction + Vector3.up * 0.5f).normalized;
            rb.AddForce(knockDir * force * knockbackForceMultiplier, ForceMode.Impulse);
        }

        SwitchToHitSprite();

        if (isPunch)
        {
            PlayPunchSound();
        }

        PlayHitSound();
        PlayPunchSound();
    }

    private void PlayHitSound()
    {
        if (hitSounds.Length == 0 || audioSource == null) return;

        audioSource.PlayOneShot(hitSounds[currentHitSound]);
        currentHitSound++;
        if (currentHitSound >= hitSounds.Length)
            currentHitSound = 0;
    }

    private void PlayPunchSound()
    {
        if (punchSounds.Length == 0 || audioSource == null) return;

        audioSource.PlayOneShot(punchSounds[currentPunchSound]);
        currentPunchSound++;
        if (currentPunchSound >= punchSounds.Length)
            currentPunchSound = 0;
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
