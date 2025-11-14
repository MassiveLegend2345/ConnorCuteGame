using System.Collections;
using UnityEngine;

public class FlipOffBox : MonoBehaviour
{
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
        col.enabled = false; 
    }

    public void Activate()
    {
        if (canHit)
            StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        canHit = false;
        col.enabled = true;

        yield return new WaitForSeconds(activeTime);

        col.enabled = false; 
        canHit = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        GameManager.Instance?.AddScore(score);
        GameManager.Instance?.AddTime(timeAdd);
        EnemyKnockback ek = other.GetComponentInParent<EnemyKnockback>();
        if (ek != null)
        {
            Vector3 knockDir = (other.transform.position - transform.position).normalized + Vector3.up * 0.2f;
            ek.Knockback(knockDir, knockbackForce);
        }
    }
}
