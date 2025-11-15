using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject enemyPrefab;
    public float spawnRadius = 10f;
    public int maxEnemies = 5;
    public float spawnInterval = 3f;

    [Header("Respawn")]
    public float respawnDelay = 2f;

    [Header("Screen Flash")]
    public Image screenFlash; // Drag your Canvas Image here!

    private int currentEnemies = 0;
    private bool isSpawning = true;

    void Start()
    {
        for (int i = 0; i < maxEnemies; i++)
            SpawnEnemy();

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (currentEnemies < maxEnemies)
                SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        Vector3 spawnPos = GetRandomNavMeshPosition(transform.position, spawnRadius);
        if (spawnPos == Vector3.zero) return;

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        currentEnemies++;

        // Start tracking this enemy's life
        StartCoroutine(HandleEnemyLife(enemy));
    }

    private IEnumerator HandleEnemyLife(GameObject enemy)
    {
        EnemyHealth eh = enemy.GetComponentInChildren<EnemyHealth>();
        if (eh == null) yield break;

        // Reset health when spawning
        eh.currentHits = 0;

        // Wait until enemy is dead - FIXED VERSION
        while (!eh.IsDead())
        {
            yield return null; // Wait one frame and check again
        }

        Debug.Log("Enemy died! Processing death effects...");

        // FREEZE THE ENEMY!
        FreezeEnemy(enemy);

        // Reward player
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(eh.scorePerKill);
            GameManager.Instance.AddTime(eh.timePerKill);
        }

        // Player boost
        FPSController player = FindObjectOfType<FPSController>();
        if (player != null)
        {
            Debug.Log("Starting player boost...");
            StartCoroutine(PlayerKillBoost(player, eh));
        }

        // Show points popup
        if (eh.pointsPopupPrefab != null && PointsCanvas.instance != null)
        {
            GameObject popup = Instantiate(eh.pointsPopupPrefab, PointsCanvas.instance.transform);
            PointsPopup pp = popup.GetComponent<PointsPopup>();
            if (pp != null)
                pp.Setup(eh.pointsText, enemy.transform.position + Vector3.up * 2f, Camera.main);
        }

        // SCREEN FLASH
        if (screenFlash != null)
        {
            Debug.Log("Starting screen flash...");
            StartCoroutine(DoScreenFlash(0.7f));
        }

        yield return new WaitForSeconds(respawnDelay);

        // Destroy dead enemy
        Destroy(enemy);
        currentEnemies--;

        // Spawn a fresh one
        SpawnEnemy();
    }

    private void FreezeEnemy(GameObject enemy)
    {
        // Freeze Rigidbody
        Rigidbody rb = enemy.GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Disable AI
        NavMeshAgent agent = enemy.GetComponentInChildren<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        // Disable collider
        Collider col = enemy.GetComponentInChildren<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Switch to hit sprite
        EnemyKnockback ek = enemy.GetComponentInChildren<EnemyKnockback>();
        if (ek != null)
        {
            if (ek.spriteOne != null) ek.spriteOne.SetActive(false);
            if (ek.spriteTwo != null) ek.spriteTwo.SetActive(true);
        }
    }

    private IEnumerator PlayerKillBoost(FPSController player, EnemyHealth eh)
    {
        Debug.Log("Player boost started!");

        if (player.boostEffect != null)
            player.boostEffect.SetActive(true);

        float origRun = player.runningSpeed;
        float origWalk = player.walkingSpeed;
        float origPunch = player.punchForce;

        player.runningSpeed *= eh.killSpeedBoost;
        player.walkingSpeed *= eh.killSpeedBoost;
        player.punchForce *= eh.killPunchBoost;

        Debug.Log($"Speed boosted! Run: {player.runningSpeed}, Walk: {player.walkingSpeed}");

        // Camera shake
        if (player.playerCamera != null)
            StartCoroutine(CameraShake(player.playerCamera.transform, 0.2f, 0.1f));

        yield return new WaitForSeconds(eh.boostDuration);

        player.runningSpeed = origRun;
        player.walkingSpeed = origWalk;
        player.punchForce = origPunch;

        if (player.boostEffect != null)
            player.boostEffect.SetActive(false);

        Debug.Log("Player boost ended!");
    }

    private IEnumerator DoScreenFlash(float duration)
    {
        if (screenFlash == null) yield break;

        Debug.Log("Screen flash started!");

        screenFlash.gameObject.SetActive(true);
        Color flashColor = screenFlash.color;
        flashColor.a = 0.5f;
        screenFlash.color = flashColor;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            flashColor.a = Mathf.Lerp(0.5f, 0f, elapsed / duration);
            screenFlash.color = flashColor;
            elapsed += Time.deltaTime;
            yield return null;
        }

        flashColor.a = 0f;
        screenFlash.color = flashColor;
        screenFlash.gameObject.SetActive(false);

        Debug.Log("Screen flash ended!");
    }

    private IEnumerator CameraShake(Transform cam, float duration, float magnitude)
    {
        Vector3 orig = cam.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            cam.localPosition = orig + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.localPosition = orig;
    }

    private Vector3 GetRandomNavMeshPosition(Vector3 center, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * radius;
            randomPoint.y = 0;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, radius, NavMesh.AllAreas))
                return hit.position;
        }

        return Vector3.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}