using UnityEngine;

public class MoverBetweenPoints : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform pointA;            // First target point
    public Transform pointB;            // Second target point
    public float moveSpeed = 2f;        // Movement speed
    public bool pingPong = true;        // Whether it goes back and forth

    [Header("Rotation Settings")]
    public Vector3 rotationSpeed = new Vector3(0f, 90f, 0f);  // Degrees per second

    private Transform currentTarget;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Please assign Point A and Point B in the inspector!", this);
            enabled = false;
            return;
        }

        currentTarget = pointB;
    }

    void Update()
    {
        // Rotate continuously
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // Move toward the current target
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);

        // If close enough to the target, switch direction
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.05f)
        {
            if (pingPong)
            {
                currentTarget = currentTarget == pointA ? pointB : pointA;
            }
            else
            {
                currentTarget = pointB;
            }
        }
    }

    // Optional: visualize movement path in editor
    void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
