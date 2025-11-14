using System.Collections;
using UnityEngine;

public class FlipOffBox : MonoBehaviour
{
    [Header("Flip Off Settings")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float activeTime = 0.1f;
    [SerializeField] private Collider hitboxCollider;  // assign in inspector

    [Header("Score & Timer")]
    [SerializeField] private int scoreAmount = 5;
    [SerializeField] private float timeAmount = 1f;

    public void Activate()
    {
        // Make sure we run the coroutine even if the visual or collider is disabled
        StartCoroutine(HandleFlipOff());
    }

    private IEnumerator HandleFlipOff()
    {
        // Enable the collider for the duration
        if (hitboxCollider != null) hitboxCollider.enabled = true;

        yield return new WaitForSeconds(activeTime);

        // Detect enemies manually
        Collider[] hits = Physics.OverlapBox(hitboxCollider.transform.position, hitboxCollider.bounds.extents);
        foreach (Collider other in hits)
        {
            if (other.CompareTag("Enemy"))
            {
                GameManager.Instance?.AddScore(scoreAmount);
                GameManager.Instance?.AddTime(timeAmount);

                EnemyKnockback ek = other.GetComponentInParent<EnemyKnockback>();
                if (ek != null)
                {
                    Vector3 knockDir = (other.transform.position - transform.position).normalized;
                    ek.Knockback(knockDir, knockbackForce);
                }
            }
        }

        // Disable collider again
        if (hitboxCollider != null) hitboxCollider.enabled = false;
    }
}
