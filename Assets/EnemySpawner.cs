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

    [Header("Kill Rewards")]
    public int scorePerKill = 10;
    public float timePerKill = 2f;
    public float killSpeedBoost = 1.5f;
    public float killPunchBoost = 1.5f;
    public float boostDuration = 3f;

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

        // RESET THE ENEMY PROPERLY
        ResetEnemy(enemy);

        StartCoroutine(HandleEnemyLife(enemy));
    }

    // NEW: Properly reset enemy state
    private void ResetEnemy(GameObject enemy)
    {
        // Reset health
        EnemyHealth eh = enemy.GetComponentInChildren<EnemyHealth>();
        if (eh != null)
        {
            eh.currentHits = 0; // Make sure health is reset
        }

        // Reset knockback sprites
        EnemyKnockback ek = enemy.GetComponentInChildren<EnemyKnockback>();
        if (ek != null)
        {
            if (ek.spriteOne != null) ek.spriteOne.SetActive(true);
            if (ek.spriteTwo != null) ek.spriteTwo.SetActive(false);
            if (ek.explosionSprite != null) ek.explosionSprite.SetActive(false);
        }

        // Enable components
        Rigidbody rb = enemy.GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }

        NavMeshAgent agent = enemy.GetComponentInChildren<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = true;
        }

        Collider col = enemy.GetComponentInChildren<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    private IEnumerator HandleEnemyLife(GameObject enemy)
    {
        EnemyHealth eh = enemy.GetComponentInChildren<EnemyHealth>();
        if (eh == null) yield break;

        // Wait until enemy is dead
        while (!eh.IsDead())
        {
            yield return null;
        }

        Debug.Log("Enemy died! Triggering explosion...");

        // TRIGGER EXPLOSION
        EnemyKnockback ek = enemy.GetComponentInChildren<EnemyKnockback>();
        if (ek != null)
        {
            ek.TriggerExplosion();
        }

        // REWARD PLAYER
        GameManager.Instance?.AddScore(scorePerKill);
        GameManager.Instance?.AddTime(timePerKill);

        // PLAYER BOOST
        if (player != null)
        {
            StartCoroutine(PlayerKillBoost(player));
        }

        // SCREEN FLASH
        if (screenFlash != null)
        {
            StartCoroutine(DoScreenFlash());
        }

        // SCREEN SHAKE
        if (player != null && player.playerCamera != null)
        {
            StartCoroutine(CameraShake(player.playerCamera.transform, 0.2f, 0.1f));
        }

        yield return new WaitForSeconds(respawnDelay);

        Destroy(enemy);
        currentEnemies--;
        SpawnEnemy();
    }

    private IEnumerator PlayerKillBoost(FPSController player)
    {
        Debug.Log("Starting player boost!");

        // Stop any existing boost
        if (currentBoostCoroutine != null)
        {
            StopCoroutine(currentBoostCoroutine);
        }

        // Store original values
        float originalRun = player.runningSpeed;
        float originalWalk = player.walkingSpeed;
        float originalPunch = player.punchForce;

        // Apply boost
        player.runningSpeed *= killSpeedBoost;
        player.walkingSpeed *= killSpeedBoost;
        player.punchForce *= killPunchBoost;

        // Visual effect
        if (player.boostEffect != null)
            player.boostEffect.SetActive(true);

        Debug.Log($"Speed boosted! Run: {player.runningSpeed}, Walk: {player.walkingSpeed}");

        // Wait for boost duration
        yield return new WaitForSeconds(boostDuration);

        // Reset to original values
        player.runningSpeed = originalRun;
        player.walkingSpeed = originalWalk;
        player.punchForce = originalPunch;

        // Visual effect off
        if (player.boostEffect != null)
            player.boostEffect.SetActive(false);

        currentBoostCoroutine = null;
        Debug.Log("Player boost ended!");
    }

    private IEnumerator DoScreenFlash()
    {
        if (screenFlash == null) yield break;

        screenFlash.gameObject.SetActive(true);
        Color flashColor = screenFlash.color;
        flashColor.a = 0.5f;
        screenFlash.color = flashColor;

        float elapsed = 0f;
        float duration = 0.7f;

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