using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; // ADD THIS

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text highScoreText;
    public Button startButton;    // Drag your start button here
    public Button quitButton;     // Drag your quit button here

    private void Start()
    {
        // Show high score if saved
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = "High Score: " + highScore;
        }

        // Make sure time scale is normal
        Time.timeScale = 1f;

        // Connect buttons to functions
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene"); // Change "Game" to your game scene name
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void Update()
    {
        // Optional: Press Escape to quit from main menu too
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

        // Optional: Press Enter to start game
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            StartGame();
        }
    }
}