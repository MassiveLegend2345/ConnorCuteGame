using System.Collections;
using UnityEngine;

public class PunchyBox : MonoBehaviour
{
    [SerializeField] private float punchForce = 15f;
    [SerializeField] private float recoilForce = 10f;
    [SerializeField] private float punchDelay = 0.5f;
    [SerializeField] private FPSController player;

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
