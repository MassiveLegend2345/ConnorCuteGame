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
    public Image screenFlash;

    private int currentEnemies = 0;
    private bool isSpawning = true;
    private Coroutine currentBoostCoroutine;
    private FPSController player;

    void Start()
    {
        player = FindObjectOfType<FPSController>();
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

        // ENSURE the new enemy is properly set up for knockback
        SetupNewEnemy(enemy);

        StartCoroutine(HandleEnemyLife(enemy));
    }

    // NEW: Ensure new enemies are properly set up for knockback
    private void SetupNewEnemy(GameObject enemy)
    {
        // Get the Rigidbody from the Hitbox child
        Rigidbody rb = enemy.GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // CRITICAL: Must be false for knockback
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Enable NavMeshAgent
        NavMeshAgent agent = enemy.GetComponentInChildren<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        // Enable Collider
        Collider col = enemy.GetComponentInChildren<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        // Reset sprite to normal state
        EnemyKnockback ek = enemy.GetComponentInChildren<EnemyKnockback>();
        if (ek != null)
        {
            if (ek.spriteOne != null) ek.spriteOne.SetActive(true);
            if (ek.spriteTwo != null) ek.spriteTwo.SetActive(false);
            ek.RefreshRigidbody(); // Refresh the Rigidbody reference
        }

        // Reset health
        EnemyHealth eh = enemy.GetComponentInChildren<EnemyHealth>();
        if (eh != null)
        {
            eh.currentHits = 0;
        }

        Debug.Log("New enemy spawned and set up for knockback!");
    }

    private IEnumerator HandleEnemyLife(GameObject enemy)
    {
        EnemyHealth eh = enemy.GetComponentInChildren<EnemyHealth>();
        if (eh == null) yield break;

        eh.currentHits = 0;

        while (!eh.IsDead())
        {
            yield return null;
        }

        FreezeEnemy(enemy);

        // Reward player
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(eh.scorePerKill);
            GameManager.Instance.AddTime(eh.timePerKill);
        }

        // Player boost
        if (player != null)
        {
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
            StartCoroutine(DoScreenFlash(0.7f));
        }

        yield return new WaitForSeconds(respawnDelay);

        Destroy(enemy);
        currentEnemies--;
        SpawnEnemy();
    }

    private void FreezeEnemy(GameObject enemy)
    {
        Rigidbody rb = enemy.GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        NavMeshAgent agent = enemy.GetComponentInChildren<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        Collider col = enemy.GetComponentInChildren<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        EnemyKnockback ek = enemy.GetComponentInChildren<EnemyKnockback>();
        if (ek != null)
        {
            if (ek.spriteOne != null) ek.spriteOne.SetActive(false);
            if (ek.spriteTwo != null) ek.spriteTwo.SetActive(true);
        }
    }

    private IEnumerator PlayerKillBoost(FPSController player, EnemyHealth eh)
    {
        if (currentBoostCoroutine != null)
        {
            StopCoroutine(currentBoostCoroutine);
            ResetPlayerSpeed(player);
        }

        if (player.boostEffect != null)
            player.boostEffect.SetActive(true);

        float baseRunSpeed = 11.5f;
        float baseWalkSpeed = 7.5f;
        float basePunchForce = 15f;

        player.runningSpeed = baseRunSpeed * eh.killSpeedBoost;
        player.walkingSpeed = baseWalkSpeed * eh.killSpeedBoost;
        player.punchForce = basePunchForce * eh.killPunchBoost;

        if (player.playerCamera != null)
            StartCoroutine(CameraShake(player.playerCamera.transform, 0.2f, 0.1f));

        currentBoostCoroutine = StartCoroutine(BoostTimer(player, eh.boostDuration, baseRunSpeed, baseWalkSpeed, basePunchForce));
        yield return currentBoostCoroutine;
    }

    private IEnumerator BoostTimer(FPSController player, float duration, float baseRun, float baseWalk, float basePunch)
    {
        yield return new WaitForSeconds(duration);

        player.runningSpeed = baseRun;
        player.walkingSpeed = baseWalk;
        player.punchForce = basePunch;

        if (player.boostEffect != null)
            player.boostEffect.SetActive(false);

        currentBoostCoroutine = null;
    }

    private void ResetPlayerSpeed(FPSController player)
    {
        player.runningSpeed = 11.5f;
        player.walkingSpeed = 7.5f;
        player.punchForce = 15f;

        if (player.boostEffect != null)
            player.boostEffect.SetActive(false);
    }

    private IEnumerator DoScreenFlash(float duration)
    {
        if (screenFlash == null) yield break;

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