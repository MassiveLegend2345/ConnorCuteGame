using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlipOffBox : MonoBehaviour
{
    public float knockForce = 5f;
    public int scoreAmount = 5;
    public float timeAmount = 1f;
    public AudioClip[] hitSounds;
    public AudioSource audioSource;
    public float cooldown = 2f;

    public Image cooldownOverlay;

    public AudioClip readySound;

    private bool canFlip = true;
    private int currentSoundIndex = 0;
    private Collider col;
    private bool isOnCooldown = false;

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.enabled = false;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(true);
            cooldownOverlay.color = new Color(1, 1, 1, 1f);
        }
    }

    public void Activate()
    {
        if (canFlip && !isOnCooldown)
            StartCoroutine(FlipRoutine());
    }

    private IEnumerator FlipRoutine()
    {
        canFlip = false;
        isOnCooldown = true;

        if (cooldownOverlay != null)
            cooldownOverlay.color = new Color(1, 1, 1, 0f);

        col.enabled = true;

        yield return new WaitForSeconds(0.1f);

        Collider[] hits = Physics.OverlapBox(transform.position, transform.localScale / 2f);
        bool hitSomething = false;

        foreach (Collider other in hits)
        {
            if (other.CompareTag("Enemy"))
            {
                hitSomething = true;
                GameManager.Instance?.AddScore(scoreAmount);
                GameManager.Instance?.AddTime(timeAmount);

                EnemyKnockback ek = other.GetComponentInParent<EnemyKnockback>();
                if (ek != null)
                {
                    Vector3 knockDir = (other.transform.position - transform.position).normalized;
                    ek.Knockback(knockDir, knockForce);
                }
            }
        }

        if (hitSomething) PlayHitSound();
        col.enabled = false;

        float timer = 0f;
        while (timer < cooldown)
        {
            timer += Time.deltaTime;
            float progress = timer / cooldown;

            if (cooldownOverlay != null)
            {
                float alpha = Mathf.Lerp(0f, 1f, progress);
                cooldownOverlay.color = new Color(1, 1, 1, alpha);
            }

            yield return null;
        }

        if (cooldownOverlay != null)
            cooldownOverlay.color = new Color(1, 1, 1, 1f);

        PlayReadySound();

        canFlip = true;
        isOnCooldown = false;
    }

    private void PlayHitSound()
    {
        if (hitSounds.Length == 0 || audioSource == null) return;
        audioSource.PlayOneShot(hitSounds[currentSoundIndex]);
        currentSoundIndex = (currentSoundIndex + 1) % hitSounds.Length;
    }

    private void PlayReadySound()
    {
        if (readySound != null && audioSource != null)
        {
            Debug.Log("Playing ready sound!");
            audioSource.PlayOneShot(readySound);
        }
        else
        {
            Debug.LogWarning("Ready sound not set up properly!");
            if (readySound == null) Debug.LogWarning("ReadySound is null");
            if (audioSource == null) Debug.LogWarning("AudioSource is null");
        }
    }
}