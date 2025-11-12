using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    [Header("Face Player Settings")]
    public bool facePlayer = true;
    public float rotationSpeed = 5f;

    private Transform player;
    private Transform cameraTransform;

    private void Start()
    {
        // Find the player by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // Get the main camera
        cameraTransform = Camera.main.transform;

        if (player == null)
        {
            Debug.LogWarning("Player not found! Make sure your player has the 'Player' tag.");
        }
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
        // Always face the camera (billboarding)
        if (cameraTransform != null)
        {
            transform.LookAt(transform.position + cameraTransform.forward, cameraTransform.up);
        }
    }

    private void FaceTowardsPlayer()
    {
        // Calculate direction to player (ignore Y axis for horizontal rotation)
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Keep rotation horizontal only

        // Only rotate if we have a meaningful direction
        if (directionToPlayer.magnitude > 0.1f)
        {
            // Calculate the target rotation
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // Smoothly rotate towards the player
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    [ContextMenu("Test Face Player")]
    public void TestFacePlayer()
    {
        if (player != null)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0;
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
            Debug.Log("Instantly faced player");
        }
    }
}