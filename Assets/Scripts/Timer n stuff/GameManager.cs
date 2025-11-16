using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float timeRemaining = 60f;
    public int score = 0;
    public bool gameEnded = false;
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public GameObject gameOverScreen;
    public TMP_Text finalScoreText;
    public GameObject exitInstructions; 

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


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}