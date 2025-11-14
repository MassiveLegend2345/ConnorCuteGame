using System.Collections;
using UnityEngine;

public class PunchyBox : MonoBehaviour
{
    [Header("Punch Settings")]
    [SerializeField] private float punchForce = 15f;
    [SerializeField] private float recoilForce = 10f;
    [SerializeField] private float punchDelay = 0.5f;
    [SerializeField] private FPSController player;

    [Header("Score & Timer")]
    [SerializeField] private int scoreAmount = 1;      // Points per punch
    [SerializeField] private float timeAmount = 0.5f;  // Seconds added per punch

    public void Activate()
    {
        StartCoroutine(HandlePunch());
    }

    private IEnumerator HandlePunch()
    {
        gameObject.SetActive(true);

        yield return new WaitForSeconds(punchDelay);

        Collider[] hits = Physics.OverlapBox(transform.position, transform.localScale / 2f);
        foreach (Collider other in hits)
        {
            if (other.CompareTag("Enemy"))
            {
                // Add score and time
                GameManager.Instance?.AddScore(scoreAmount);
                GameManager.Instance?.AddTime(timeAmount);

                EnemyKnockback ek = other.GetComponentInParent<EnemyKnockback>();
                if (ek != null)
                {
                    Vector3 knockDir = (other.transform.position - transform.position).normalized;
                    ek.Knockback(knockDir, punchForce);
                }
            }
            else
            {
                if (player != null)
                {
                    Vector3 recoilDir = -transform.forward * recoilForce;
                    player.ApplyRecoil(recoilDir);
                }
            }
        }

        yield return new WaitForSeconds(0.15f);
        gameObject.SetActive(false);
    }
}
