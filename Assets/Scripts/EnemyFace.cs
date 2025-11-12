using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public bool facePlayer = true;
    public float rotationSpeed = 5f;
    private Transform player;
    private Transform cameraTransform;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (facePlayer && player != null)
        {
            FaceTowardsPlayer();
        }
    }

    private void LateUpdate()
    {
        if (cameraTransform != null)
        {
            transform.LookAt(transform.position + cameraTransform.forward, cameraTransform.up);
        }
    }

    private void FaceTowardsPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; 

        if (directionToPlayer.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void TestFacePlayer()
    {
        if (player != null)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0;
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }
    }
}