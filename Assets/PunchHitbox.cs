using System.Collections;
using UnityEngine;

public class Punchy : MonoBehaviour
{
    [Header("Punch Settings")]
    [SerializeField] private float punchForce = 15f;
    [SerializeField] private float recoilForce = 10f;
    [SerializeField] private float punchDelay = 0.5f; // delay before knockback applies
    [SerializeField] private FPSController player; // assign in Inspector

    private void OnTriggerEnter(Collider other)
    {
        // Start coroutine to delay the knockback
        StartCoroutine(HandlePunchHit(other));
    }

    private IEnumerator HandlePunchHit(Collider other)
    {
        yield return new WaitForSeconds(punchDelay);

        // --- ENEMY HIT ---
        if (other.CompareTag("Enemy"))
        {
            EnemyKnockback enemy = other.GetComponentInParent<EnemyKnockback>();
            if (enemy != null)
            {
                Vector3 knockDir = (other.transform.position - transform.position).normalized;
                enemy.Knockback(knockDir, punchForce);
                yield break; // skip recoil if enemy was hit
            }
        }

        // --- WALL / NON-ENEMY HIT ---
        // Only recoil player if we hit something solid
        if (!other.isTrigger)
        {
            Vector3 recoilDir = -transform.forward * recoilForce;
            player.ApplyRecoil(recoilDir);
        }
    }
}
