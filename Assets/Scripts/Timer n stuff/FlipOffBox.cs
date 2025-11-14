using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlipOffBox : MonoBehaviour
{
    public float knockForce = 5f;

    public int scoreAmount = 5;
    public float timeAmount = 1f;

    public AudioClip[] hitSounds;
    public AudioSource audioSource;

    private int currentSoundIndex = 0;
    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.enabled = false;
    }

    public void Activate()
    {
        StartCoroutine(FlipRoutine());
    }

    private IEnumerator FlipRoutine()
    {
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

        if (hitSomething)
        {
            PlayHitSound();
        }

        col.enabled = false;
    }

    private void PlayHitSound()
    {
        if (hitSounds.Length == 0 || audioSource == null) return;

        audioSource.clip = hitSounds[currentSoundIndex];
        audioSource.Play();

        currentSoundIndex++;
        if (currentSoundIndex >= hitSounds.Length)
            currentSoundIndex = 0;
    }
}
