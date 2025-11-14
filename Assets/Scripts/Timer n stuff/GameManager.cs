using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Runtime")]
    public float timeRemaining = 60f;
    public int score = 0;
    public bool gameEnded = false;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public GameObject gameOverScreen;
    public TMP_Text finalScoreText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        Time.timeScale = 1f;

        if (gameOverScreen != null) gameOverScreen.SetActive(false);
    }

    private void Update()
    {
        if (gameEnded) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndGame();
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (timerText != null)
            timerText.text = "TIME: " + Mathf.CeilToInt(timeRemaining);

        if (scoreText != null)
            scoreText.text = "SCORE: " + score;
    }

    public void AddScore(int amount)
    {
        score += amount;
        // Optional: can add visual feedback here
    }

    public void AddTime(float amount)
    {
        timeRemaining += amount;
    }

    public void EndGame()
    {
        if (gameEnded) return;

        gameEnded = true;

        if (gameOverScreen != null) gameOverScreen.SetActive(true);
        if (finalScoreText != null) finalScoreText.text = "Your Score: " + score;

        Time.timeScale = 0f;

        Debug.Log("GAME OVER — FINAL SCORE: " + score);
    }
}
