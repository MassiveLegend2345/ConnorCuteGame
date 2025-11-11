using UnityEngine;

public class Punchy : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
                rb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
            }
        }
    }
}