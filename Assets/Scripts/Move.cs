using UnityEngine;

public class MoverBetweenPoints : MonoBehaviour
{
    public Transform pointA;        
    public Transform pointB;         
    public float moveSpeed = 2f;       
    public bool pingPong = true;        

    public Vector3 rotationSpeed = new Vector3(0f, 90f, 0f);  

    private Transform currentTarget;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("WROOOOOONG", this);
            enabled = false;
            return;
        }

        currentTarget = pointB;
    }

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);
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
    void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
