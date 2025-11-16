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
    public float cooldown = 2f; // Increased cooldown to prevent spam

    [Header("Cooldown Visual")]
    public Image cooldownOverlay; // Drag a semi-transparent image here

    private bool canFlip = true;
    private int currentSoundIndex = 0;
    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.enabled = false;

        // Hide cooldown overlay at start
        if (cooldownOverlay != null)
            cooldownOverlay.gameObject.SetActive(false);
    }

    public void Activate()
    {
        if (canFlip)
            StartCoroutine(FlipRoutine());
    }

    private IEnumerator FlipRoutine()
    {
        canFlip = false;

        // SHOW COOLDOWN VISUAL
        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(true);
            cooldownOverlay.color = new Color(1, 1, 1, 0.7f); // Semi-transparent white
        }

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

        // COOLDOWN PERIOD WITH VISUAL FEEDBACK
        float timer = cooldown;
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // Fade out the overlay during cooldown
            if (cooldownOverlay != null)
            {
                float alpha = Mathf.Lerp(0f, 0.7f, timer / cooldown);
                cooldownOverlay.color = new Color(1, 1, 1, alpha);
            }

            yield return null;
        }

        // HIDE COOLDOWN VISUAL
        if (cooldownOverlay != null)
            cooldownOverlay.gameObject.SetActive(false);

        canFlip = true;
    }

    private void PlayHitSound()
    {
        if (hitSounds.Length == 0 || audioSource == null) return;
        audioSource.clip = hitSounds[currentSoundIndex];
        audioSource.Play();
        currentSoundIndex = (currentSoundIndex + 1) % hitSounds.Length;
    }
}