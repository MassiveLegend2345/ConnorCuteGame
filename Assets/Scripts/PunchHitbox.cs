using System.Collections;
using UnityEngine;

public class PunchyBox : MonoBehaviour
{
    [SerializeField] private float punchForce = 15f;
    [SerializeField] private float recoilForce = 10f;
    [SerializeField] private float punchDelay = 0.5f;
    [SerializeField] private FPSController player;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("=== PUNCH TRIGGERED ===");
        Debug.Log($"Hit object: {other.name}");
        Debug.Log($"Tag: {other.tag}");
        Debug.Log($"IsTrigger: {other.isTrigger}");
        Debug.Log($"Layer: {LayerMask.LayerToName(other.gameObject.layer)}");

        Transform p = other.transform;
        int depth = 0;
        while (p != null && depth < 5)
        {
            Debug.Log($"Parent[{depth}] = {p.name}");
            p = p.parent;
            depth++;
        }

        EnemyKnockback ek = other.GetComponentInParent<EnemyKnockback>();
        Debug.Log($"EnemyKnockback found?: {ek}");

        StartCoroutine(HandlePunchHit(other, ek));
    }

    private IEnumerator HandlePunchHit(Collider other, EnemyKnockback ek)
    {
        yield return new WaitForSeconds(punchDelay);

        // Did we hit an Enemy-tagged collider?
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Tag matched Enemy.");

            if (ek != null)
            {
                Debug.Log("Calling Knockback...");
                Vector3 knockDir = (other.transform.position - transform.position).normalized;
                ek.Knockback(knockDir, punchForce);
                yield break;
            }
            else
            {
                Debug.Log("EnemyKnockback NOT found in parent chain.");
            }
        }
        else
        {
            Debug.Log("Tag did NOT match Enemy.");
        }

        // Fallback: recoil
        if (!other.isTrigger)
        {
            Debug.Log("Applying recoil to player...");
            Vector3 recoilDir = -transform.forward * recoilForce;
            player.ApplyRecoil(recoilDir);
        }
    }
}
