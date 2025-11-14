using System.Collections;
using UnityEngine;

public class FlipOffBox : MonoBehaviour
{
    [Header("Flip-Off Settings")]
    public float activeTime = 0.15f;
    public float knockbackForce = 5f;
    public int score = 5;
    public float timeAdd = 1f;

    private Collider col;
    private bool canHit = true;

    private void Awake()
    {
        col = GetComponent<Collider>();
        if (!col) col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.enabled = false; // start disabled
    }

    public void Activate()
    {
        if (canHit)
            StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        canHit = false;
        col.enabled = true; // enable collider

        yield return new WaitForSeconds(activeTime);

        col.enabled = false; // disable collider
        canHit = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        // Add score and time
        GameManager.Instance?.AddScore(score);
        GameManager.Instance?.AddTime(timeAdd);

        // Knockback
        EnemyKnockback ek = other.GetComponentInParent<EnemyKnockback>();
        if (ek != null)
        {
            Vector3 knockDir = (other.transform.position - transform.position).normalized + Vector3.up * 0.2f;
            ek.Knockback(knockDir, knockbackForce);
        }
    }
}
