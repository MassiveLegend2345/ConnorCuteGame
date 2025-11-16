using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    public GameObject exitInstructions; // Add this UI text for instructions

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
        if (exitInstructions != null) exitInstructions.SetActive(false);
    }

    private void Update()
    {
        if (gameEnded)
        {
            // Check for input when game is over
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ReturnToMainMenu();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitGame();
            }
            return;
        }

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
        if (exitInstructions != null) exitInstructions.SetActive(true);

        Time.timeScale = 0f;

        Debug.Log("GAME OVER — FINAL SCORE: " + score);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

        // For testing in editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}