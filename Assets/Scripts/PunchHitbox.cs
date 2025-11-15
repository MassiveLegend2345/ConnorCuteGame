using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PunchyBox : MonoBehaviour
{
    [Header("Punch Settings")]
    public float punchForce = 15f;
    public float recoilForce = 8f;
    public float activeWindow = 0.12f; // how long the collider stays enabled when Activate() is called

    [Header("Score & Timer")]
    public int scoreAmount = 1;
    public float timeAmount = 0.5f;

    [Header("Punch Audio")]
    public AudioSource punchAudioSource; // always-active object
    public AudioClip[] punchClips;

    [Header("Player Reference")]
    public FPSController player;

    private Collider col;
    private int clipIndex = 0;

    // track who we've hit during the current punch activation
    private HashSet<int> hitThisActivation = new HashSet<int>();
    private bool active = false;

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
        col.enabled = false;
    }

    // Call this when the punch animation/attack starts
    public void Activate()
    {
        // reset hits from previous activation
        hitThisActivation.Clear();
        active = true;
        col.enabled = true;
        StopCoroutine(nameof(DisableAfterWindow));
        StartCoroutine(DisableAfterWindow());
        Debug.Log("[PunchyBox] Activated.");
    }

    private IEnumerator DisableAfterWindow()
    {
        yield return new WaitForSeconds(activeWindow);
        col.enabled = false;
        active = false;
        Debug.Log("[PunchyBox] Deactivated.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active)
        {
            // If the punch window isn't active, ignore (helps catch mistakes)
            Debug.Log("[PunchyBox] Triggered while not active by: " + other.name);
            return;
        }

        // Quick tag check - skip anything not enemy-tagged
        if (!other.CompareTag("Enemy"))
        {
            // we may be hitting non-enemy colliders on the enemy object; still try parent check below
            // but early return would skip those - so don't return here. We'll continue to check for EnemyHealth.
            // Debug.Log("[PunchyBox] Collider not tagged Enemy: " + other.name);
        }

        // prevent hitting same collider/transform more than once this activation
        int id = other.transform.GetInstanceID();
        if (hitThisActivation.Contains(id))
        {
            Debug.Log("[PunchyBox] Already hit this object this activation: " + other.name);
            return;
        }
        hitThisActivation.Add(id);

        Debug.Log("[PunchyBox] Punch hit: " + other.name + " (active)");

        // Score & time
        GameManager.Instance?.AddScore(scoreAmount);
        GameManager.Instance?.AddTime(timeAmount);

        // Knockback -- prefer component on the same object or parent
        EnemyKnockback ek = other.GetComponentInParent<EnemyKnockback>();
        if (ek == null)
            ek = other.GetComponent<EnemyKnockback>(); // fallback
        if (ek == null)
            ek = other.transform.root.GetComponentInChildren<EnemyKnockback>(); // fallback

        Vector3 knockDir = (other.transform.position - transform.position).normalized;
        if (ek != null)
        {
            ek.Knockback(knockDir, punchForce, true); // pass isPunch = true
            Debug.Log("[PunchyBox] Applied knockback to: " + ek.name);
        }
        else
        {
            Debug.Log("[PunchyBox] No EnemyKnockback found on: " + other.name);
        }

        // Take damage: try several lookup strategies so we don't miss it
        EnemyHealth eh = other.GetComponentInParent<EnemyHealth>();
        if (eh == null)
            eh = other.GetComponent<EnemyHealth>();
        if (eh == null)
            eh = other.transform.root.GetComponentInChildren<EnemyHealth>();

        if (eh != null)
        {
            eh.TakeHit();
            Debug.Log("[PunchyBox] Called TakeHit on: " + eh.name);
        }
        else
        {
            Debug.LogWarning("[PunchyBox] EnemyHealth NOT found for: " + other.name + " | parent: " + (other.transform.parent ? other.transform.parent.name : "null") + " | root: " + other.transform.root.name);
        }

        // Play sound local to the player if provided
        if (punchAudioSource != null && punchClips.Length > 0)
        {
            punchAudioSource.PlayOneShot(punchClips[clipIndex]);
            clipIndex = (clipIndex + 1) % punchClips.Length;
        }

        // Apply recoil to player if needed
        if (player != null)
        {
            Vector3 recoilDir = -transform.forward * recoilForce;
            player.ApplyRecoil(recoilDir);
        }
    }
}
