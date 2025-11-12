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
        StartCoroutine(HandlePunchHit(other));
    }

    private IEnumerator HandlePunchHit(Collider other)
    {
        yield return new WaitForSeconds(punchDelay);
        if (other.CompareTag("Enemy"))
        {
            EnemyKnockback enemy = other.GetComponentInParent<EnemyKnockback>();
            if (enemy != null)
            {
                Vector3 knockDir = (other.transform.position - transform.position).normalized;
                enemy.Knockback(knockDir, punchForce);
                yield break;
            }
        }

        if (!other.isTrigger)
        {
            Vector3 recoilDir = -transform.forward * recoilForce;
            player.ApplyRecoil(recoilDir);
        }
    }
}
