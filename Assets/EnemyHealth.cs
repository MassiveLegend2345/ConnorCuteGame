using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHits = 3;
    private int currentHits = 0;

    [Header("Respawn")]
    public Transform respawnPoint;
    public float respawnDelay = 1f;

    [Header("Kill Rewards")]
    public int scorePerKill = 10;
    public float timePerKill = 2f;

    [Header("Player Boost")]
    public float killSpeedBoost = 1.5f;
    public float killPunchBoost = 1.5f;
    public float boostDuration = 3f;

    [Header("Visuals")]
    public GameObject pointsPopupPrefab;  // assign PointsPopup prefab
    public string pointsText = "+10";
    public Image screenFlash; // assign full-screen UI Image (red/orange) for flash
    public float flashDuration = 0.3f;

    [Header("Components")]
    private Rigidbody rb;
    private NavMeshAgent agent;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        col = GetComponent<Collider>();
    }

    public void TakeHit()
    {
        currentHits++;

        if (currentHits >= maxHits)
            StartCoroutine(RespawnEnemy());
    }

    private IEnumerator RespawnEnemy()
    {
        // Reward player
        GameManager.Instance?.AddScore(scorePerKill);
        GameManager.Instance?.AddTime(timePerKill);

        // Spawn PointsPopup on canvas
        if (pointsPopupPrefab != null && PointsCanvas.instance != null)
        {
            GameObject popup = Instantiate(pointsPopupPrefab, PointsCanvas.instance.transform);
            PointsPopup pp = popup.GetComponent<PointsPopup>();
            if (pp != null)
                pp.Setup(pointsText, transform.position + Vector3.up * 2f, Camera.main);
        }

        // Player boost + visuals
        FPSController player = FindObjectOfType<FPSController>();
        if (player != null)
            StartCoroutine(PlayerKillBoost(player));

        // Hide enemy components
        if (agent != null) agent.enabled = false;
        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;

        // Hide visuals
        Transform spriteParent = transform.Find("Sprites");
        if (spriteParent != null)
        {
            foreach (Transform child in spriteParent)
                child.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(respawnDelay);

        // Reset HP
        currentHits = 0;

        // Warp agent to respawn
        if (agent != null && respawnPoint != null)
            agent.Warp(respawnPoint.position);
        else if (respawnPoint != null)
            transform.position = respawnPoint.position;

        // Re-enable components
        if (agent != null) agent.enabled = true;
        if (rb != null) rb.isKinematic = false;
        if (col != null) col.enabled = true;

        // Re-enable visuals
        if (spriteParent != null)
        {
            foreach (Transform child in spriteParent)
                child.gameObject.SetActive(true);
        }
    }

    private IEnumerator PlayerKillBoost(FPSController player)
    {
        // Enable boost particle
        if (player.boostEffect != null)
            player.boostEffect.SetActive(true);

        // Boost stats
        player.runningSpeed *= killSpeedBoost;
        player.walkingSpeed *= killSpeedBoost;
        player.punchForce *= killPunchBoost;

        // Screen flash
        if (screenFlash != null)
            StartCoroutine(ScreenFlashRoutine(screenFlash, flashDuration));

        // Camera shake
        if (player.playerCamera != null)
            StartCoroutine(CameraShake(player.playerCamera.transform, 0.2f, 0.1f));

        yield return new WaitForSeconds(boostDuration);

        // Reset stats
        player.runningSpeed /= killSpeedBoost;
        player.walkingSpeed /= killSpeedBoost;
        player.punchForce /= killPunchBoost;

        // Disable particle
        if (player.boostEffect != null)
            player.boostEffect.SetActive(false);
    }

    private IEnumerator ScreenFlashRoutine(Image flashImage, float duration)
    {
        flashImage.gameObject.SetActive(true);
        float elapsed = 0f;
        Color c = flashImage.color;
        c.a = 0.5f; // max alpha
        flashImage.color = c;

        while (elapsed < duration)
        {
            c.a = Mathf.Lerp(0.5f, 0f, elapsed / duration);
            flashImage.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }

        flashImage.gameObject.SetActive(false);
    }

    private IEnumerator CameraShake(Transform cam, float duration, float magnitude)
    {
        Vector3 originalPos = cam.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            cam.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.localPosition = originalPos;
    }
}
