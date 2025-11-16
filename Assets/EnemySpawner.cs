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
    private float originalRunSpeed;
    private float originalWalkSpeed;
    private float originalPunchForce;

    void Start()
    {
        player = FindObjectOfType<FPSController>();

        // STORE ORIGINAL SPEEDS ONCE
        if (player != null)
        {
            originalRunSpeed = player.runningSpeed;
            originalWalkSpeed = player.walkingSpeed;
            originalPunchForce = player.punchForce;
        }

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

        ResetEnemy(enemy);
        StartCoroutine(HandleEnemyLife(enemy));
    }

    private void ResetEnemy(GameObject enemy)
    {
        EnemyHealth eh = enemy.GetComponentInChildren<EnemyHealth>();
        if (eh != null) eh.currentHits = 0;

        EnemyKnockback ek = enemy.GetComponentInChildren<EnemyKnockback>();
        if (ek != null)
        {
            if (ek.spriteOne != null) ek.spriteOne.SetActive(true);
            if (ek.spriteTwo != null) ek.spriteTwo.SetActive(false);
            if (ek.explosionSprite != null) ek.explosionSprite.SetActive(false);
        }

        Rigidbody rb = enemy.GetComponentInChildren<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        NavMeshAgent agent = enemy.GetComponentInChildren<NavMeshAgent>();
        if (agent != null) agent.enabled = true;

        Collider col = enemy.GetComponentInChildren<Collider>();
        if (col != null) col.enabled = true;
    }

    private IEnumerator HandleEnemyLife(GameObject enemy)
    {
        EnemyHealth eh = enemy.GetComponentInChildren<EnemyHealth>();
        if (eh == null) yield break;

        while (!eh.IsDead()) yield return null;

        EnemyKnockback ek = enemy.GetComponentInChildren<EnemyKnockback>();
        if (ek != null) ek.TriggerExplosion();

        GameManager.Instance?.AddScore(scorePerKill);
        GameManager.Instance?.AddTime(timePerKill);

        if (player != null) StartCoroutine(PlayerKillBoost(player));

        if (screenFlash != null) StartCoroutine(DoScreenFlash());

        if (player != null && player.playerCamera != null)
            StartCoroutine(CameraShake(player.playerCamera.transform, 0.2f, 0.1f));

        yield return new WaitForSeconds(respawnDelay);

        Destroy(enemy);
        currentEnemies--;
        SpawnEnemy();
    }

    private IEnumerator PlayerKillBoost(FPSController player)
    {
        // STOP ANY EXISTING BOOST FIRST
        if (currentBoostCoroutine != null)
        {
            StopCoroutine(currentBoostCoroutine);
        }

        // APPLY BOOST TO ORIGINAL SPEEDS (not current speeds)
        player.runningSpeed = originalRunSpeed * killSpeedBoost;
        player.walkingSpeed = originalWalkSpeed * killSpeedBoost;
        player.punchForce = originalPunchForce * killPunchBoost;

        if (player.boostEffect != null) player.boostEffect.SetActive(true);

        yield return new WaitForSeconds(boostDuration);

        // RESET TO ORIGINAL SPEEDS
        player.runningSpeed = originalRunSpeed;
        player.walkingSpeed = originalWalkSpeed;
        player.punchForce = originalPunchForce;

        if (player.boostEffect != null) player.boostEffect.SetActive(false);
        currentBoostCoroutine = null;
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
}