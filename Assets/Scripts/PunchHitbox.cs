using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PunchyBox : MonoBehaviour
{
    [Header("Punch Settings")]
    public float punchForce = 15f;
    public float recoilForce = 8f;

    [Header("Score & Timer")]
    public int scoreAmount = 1;
    public float timeAmount = 0.5f;

    [Header("Punch Audio")]
    public AudioSource punchAudioSource;
    public AudioClip[] punchClips;

    [Header("Player Reference")]
    public FPSController player;

    private Collider col;
    private int clipIndex = 0;

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
        col.enabled = false;
    }

    public void Activate()
    {
        col.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        GameManager.Instance?.AddScore(scoreAmount);
        GameManager.Instance?.AddTime(timeAmount);

        EnemyKnockback ek = other.GetComponentInParent<EnemyKnockback>();
        if (ek != null)
        {
            Vector3 knockDir = (other.transform.position - transform.position).normalized;
            ek.Knockback(knockDir, punchForce);
        }

        if (punchAudioSource != null && punchClips.Length > 0)
        {
            punchAudioSource.PlayOneShot(punchClips[clipIndex]);
            clipIndex = (clipIndex + 1) % punchClips.Length;
        }
        if (player != null)
        {
            Vector3 recoilDir = -transform.forward * recoilForce;
            player.ApplyRecoil(recoilDir);
        }
    }
}
