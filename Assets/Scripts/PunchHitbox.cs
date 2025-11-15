using UnityEngine;

public class Punchybox : MonoBehaviour
{
    public float punchForce = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyHealth eh = other.GetComponentInParent<EnemyHealth>();
        EnemyKnockback ek = other.GetComponentInParent<EnemyKnockback>();

        if (eh != null)
        {
            if (ek != null)
            {
                Vector3 knockDir = (other.transform.position - transform.position).normalized;
                knockDir.y = 0.2f;
                ek.Knockback(knockDir, punchForce, true);
            }

            eh.TakeHit();
            GameManager.Instance?.AddScore(1);
            GameManager.Instance?.AddTime(0.5f);
        }
    }
}
